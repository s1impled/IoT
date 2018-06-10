using RET.Interfaces;
using System;
using System.Diagnostics;
using Windows.Devices.Gpio;

namespace RET
{
    public sealed class RelayAppliance : IGpioAppliance, IDisposable
    {
        private const int RelayPin = 4;
        private GpioPin relay;

        public RelayAppliance()
        {
            this.GpioPin = RelayAppliance.RelayPin;
        }

        public int GpioPin { get; private set; }

        public void Initialize()
        {
            this.CheckDisposed();

            GpioController gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                relay = null;
                Debug.WriteLine("There is no GPIO controller on this device.");
                return;
            }

            this.relay = gpio.OpenPin(this.GpioPin);
            this.relay.Write(GpioPinValue.Low);
            this.relay.SetDriveMode(GpioPinDriveMode.Output);

            Debug.WriteLine(string.Format("GPIO Initalized Correctly for Pin {0}.", this.GpioPin));
        }

        public void TurnApplianceOff()
        {
            this.CheckDisposed();

            Debug.WriteLine("Relay Appliance Off");

            this.relay.Write(GpioPinValue.Low);
        }

        public void TurnApplianceOn()
        {
            this.CheckDisposed();

            Debug.WriteLine("Relay Appliance On");

            if (this.relay.Read() != GpioPinValue.High)
            {
                this.relay.Write(GpioPinValue.High);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void CheckDisposed()
        {
            if (this.disposedValue)
            {
                throw new ObjectDisposedException(this.GetType().Name);
            }
        }

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.TurnApplianceOff();
                    this.relay.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~RelayAppliance() {
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
