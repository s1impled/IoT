using BMP180.I2C;
using Newtonsoft.Json;
using RET.Interfaces;
using ServerSentEvent;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace RET
{
    public sealed class StartupTask : IBackgroundTask, IDisposable
    {
        private const string authCode = "secret";
        private NestData nestJson;
        private Thermostat kitchen;

        private IDisplayManager display;
        private BMP180Device tempSensor;
        private IGpioAppliance relayAppliance;

        private EventSource nestApi;
        private CancellationToken hcfToken;

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            this.CheckDisposed();

            //
            // Create the deferral by requesting it from the task instance.
            //
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            this.hcfToken = new CancellationToken();

            this.display = new DisplayManager();
            await this.display.Initialize();
            this.display.WriteLine("Display Initalized.");

            this.tempSensor = new BMP180Device();
            await this.tempSensor.Initialize();

            this.relayAppliance = new RelayAppliance();
            this.relayAppliance.Initialize();

            this.nestApi = new EventSource(new Uri("https://developer-api.nest.com/"), StartupTask.authCode);
            this.nestApi.EventReceived += new EventHandler<ServerSentEvent.Events.ServerSentEventReceivedEventArgs>(async (o, e) => {
                if (e.Message.EventType == "keep-alive")
                {
                    Debug.WriteLine("EventSource: Keep-Alive Event Received");
                }
                else
                {
                    Debug.WriteLine(e.Message.Data);
                    if (!string.IsNullOrEmpty(e.Message.Data))
                    {
                        try
                        {
                            this.nestJson = JsonConvert.DeserializeObject<NestReadResponse>(e.Message.Data).Data;
                            await this.UpdateRetFromEvent();
                        }
                        catch
                        {
                            Debug.WriteLine("EventSource: Failed to parse Server Event Message Data");
                        }
                        
                    }
                }
            });
            this.nestApi.StateChanged += new EventHandler<ServerSentEvent.Events.StateChangedEventArgs>((o, e) => {
                Debug.WriteLine(string.Format("State Changed: {0}", e.State.ToString()));
            });
            this.nestApi.Start(this.hcfToken);
            
            await this.RunPollingLoop(this.hcfToken);

            //
            // Once the asynchronous method(s) are done, close the deferral.
            //
            deferral.Complete();
        }

        private async Task RunPollingLoop(CancellationToken token)
        {
            this.CheckDisposed();

            Debug.WriteLine("RET Start RunPollingLoop");

            do
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("RET Polling Loop Cancel Called");
                    break;
                }

                Debug.WriteLine("RET: Checking Event Source");
                if (this.nestApi.State == EventSourceState.CLOSED)
                {
                    Debug.WriteLine("RET: Nest SSE Stream disconnected.");
                }

                await Task.Delay(TimeSpan.FromMinutes(10));
            } while (true);

            Debug.WriteLine("RET End RunPollingLoop");
        }

        private async Task UpdateRetFromEvent()
        {
            this.CheckDisposed();

            this.kitchen = this.nestJson.Devices.Thermostats[this.nestJson.Structures.First().Value.thermostats[0]];
            Debug.WriteLine("Kitchen is reading {0}\u00b0 F", kitchen.ambient_temperature_f);
            Debug.WriteLine(string.Format("Kitchen is {0}", kitchen.hvac_state));
            Debug.WriteLine(string.Format("Kitchen mode {0}", kitchen.hvac_mode));

            // Read the local BMP180 sensor data
            BMP180SensorData sensorReading = await this.tempSensor.ReadSensorDataAsync(BMP180AccuracyMode.Standard);
            Debug.WriteLine(string.Format("RET BMP180 Sample\r\n{0}", sensorReading));


            this.display.WriteLine("Kitchen is {0}\u00b0 F\r\nRET is {1:N0}\u00b0 F\r\nKitchen mode is {2}",
                kitchen.ambient_temperature_f, sensorReading.TemperatureInFahrenheit, kitchen.hvac_mode);


            switch (this.kitchen.hvac_mode)
            {
                case "off":
                // Intentional fall thru
                case "eco":
                    if (kitchen.ambient_temperature_f < kitchen.target_temperature_f)
                    {
                        this.relayAppliance.TurnApplianceOn();
                        this.display.DisplayHeatIcon = true;
                        break;
                    }

                    // Intentional fall thru
                    goto case "heat";
                case "heat":
                    this.relayAppliance.TurnApplianceOff();
                    this.display.DisplayHeatIcon = false;
                    break;

                default:
                    Debug.WriteLine(string.Format("Unknown hvac_mode value: {0}", this.kitchen.hvac_mode));
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.relayAppliance.Dispose();
                    this.display.Dispose();
                    this.tempSensor.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~StartupTask() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        private void CheckDisposed()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
