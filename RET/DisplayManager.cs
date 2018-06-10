using RET.Interfaces;
using SSD1306.I2C;
using SSD1306.Images;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Networking.Connectivity;

namespace RET
{
    internal sealed class DisplayManager : IDisplayManager, IDisposable
    {
        private SSD1306Device display;
        private int minutesIdle;
        private Timer clock;
        private EventWaitHandle clockTicked;
        private NetworkStatusChangedEventHandler networkStatusCallback;

        private const string clockFormat = "h:mm\u0091tt";
        private const int clockColumn = 83;

        private bool displayHeatIcon;

        public DisplayManager()
        {
            this.display = new SSD1306Device();
            this.IsDisplayOn = false;
            this.minutesIdle = 0;
            this.clockTicked = new EventWaitHandle(false, EventResetMode.AutoReset);
            this.clock = new Timer(new TimerCallback(this.ClockTick), null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
            this.DisplayTimeout = TimeSpan.FromMinutes(1);

            // register for network status change notifications
            networkStatusCallback = new NetworkStatusChangedEventHandler(OnNetworkStatusChange);
            NetworkInformation.NetworkStatusChanged += networkStatusCallback;
        }

        public TimeSpan DisplayTimeout { get; set; }

        public bool IsDisplayOn { get; private set; }

        public bool DisplayHeatIcon
        {
            get {
                return this.displayHeatIcon;
            }
            set {
                this.displayHeatIcon = value;
                this.WriteStatusLine();
            }
        }

        public IAsyncOperation<bool> Initialize()
        {
            return this.InitializeHelper().AsAsyncOperation();
        }

        private async Task<bool> InitializeHelper()
        {
            await this.display.Initialize();
            this.IsDisplayOn = true;
            Task clockLoop = Task.Run(() => { this.RunClockLoop(CancellationToken.None); });
            return true;
        }

        public void TurnOff()
        {
            this.display.DisplaySleep();
            this.IsDisplayOn = false;
        }

        public void TurnOn()
        {
            if (!this.IsDisplayOn)
            {
                this.display.AwakenDisplay();
                this.minutesIdle = 0;
                this.IsDisplayOn = true;
            }
        }

        public void WriteLine(string format, params object[] args)
        {
            string message = string.Format(format, args);
            this.display.ClearDisplayBuf();
            this.WriteStatusLine();
            this.display.WriteLineDisplayBuf(message, 0, 1);
            this.display.DisplayUpdate();
            this.TurnOn();
        }

        private bool isWiFiConnected()
        {
            ConnectionProfile InternetConnectionProfile = NetworkInformation.GetInternetConnectionProfile();
            return (InternetConnectionProfile == null) ? false : InternetConnectionProfile.IsWlanConnectionProfile;
        }

        private void OnNetworkStatusChange(object sender)
        {
            this.WriteStatusLine();
        }

        private void WriteStatusLine()
        {
            if (this.isWiFiConnected())
            {
                this.display.WriteImageDisplayBuf(DisplayImages.WiFiConnected, 0, 0);
            } else {
                this.display.WriteImageDisplayBuf(DisplayImages.NotConnected, 0, 0);
            }

            if (this.DisplayHeatIcon)
            {
                this.display.WriteImageDisplayBuf(DisplayImages.FireIcon, DisplayImages.WiFiConnected.ImageWidthPx + 4, 0);
            }

            this.display.WriteLineDisplayBuf(DateTime.Now.ToString(DisplayManager.clockFormat).ToLowerInvariant(), DisplayManager.clockColumn, 0);
            this.display.DisplayUpdate();
        }

        private void ClockTick(object state)
        {
            this.clockTicked.Set();
        }

        private void RunClockLoop(CancellationToken token)
        {
            Debug.WriteLine("DisplayManager.RunClockLoop: Enter");
            int seconds = 0;
            bool showColon = true;

            do
            {
                if (token.IsCancellationRequested)
                {
                    Debug.WriteLine("DisplayManager.RunClockLoop: Cancel Requested");
                    break;
                }

                if (seconds++ == 59)
                {
                    Debug.WriteLine("DisplayManager.RunClockLoop: Minute tick {0}", this.minutesIdle);

                    // Display timeout
                    if (++this.minutesIdle >= this.DisplayTimeout.TotalMinutes)
                    {
                        if (this.IsDisplayOn)
                        {
                            this.TurnOff();
                        }
                    }
                    seconds = 0;
                }

                this.display.WriteLineDisplayBuf(DateTime.Now.ToString(showColon ? DisplayManager.clockFormat : DisplayManager.clockFormat.Replace(':', '\u0091')).ToLowerInvariant(), DisplayManager.clockColumn, 0);
                showColon = !showColon;
                this.display.DisplayUpdate();
                this.clockTicked.WaitOne();
            } while (true);

            Debug.WriteLine("DisplayManager.RunClockLoop: Exit");
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    this.display.DisplaySleep();
                    this.display.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~DisplayManager() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

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
