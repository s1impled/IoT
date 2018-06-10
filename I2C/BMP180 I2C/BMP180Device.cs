using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.I2c;

namespace BMP180.I2C
{
    public sealed class BMP180Device : IDisposable
    {
        private bool isInitalized = false;

        /* Definitions for I2C */
        private const string I2CControllerName = "I2C1";
        private I2cDevice bmp180I2c;

        #region Calibration Data Fields

        // calibration data fields for polynomials used in calculating 
        // compensated temp and pressure.
        private double x0;
        private double x1;
        private double x2;
        private double y0;
        private double y1;
        private double y2;
        private double p0;
        private double p1;
        private double p2;

        #endregion Calibration Data Fields

        public BMP180Device(byte I2cAddress = 0x77)
        {
            this.I2cAddress = I2cAddress;
            this.CalibrationData = new BMP180CalibrationData();
        }

        /// <summary>
        /// Gets the I2c Buss Address of the device
        /// </summary>
        public byte I2cAddress { get; private set; }

        /// <summary>
        /// Gets the calibration data from the device
        /// </summary>
        public BMP180CalibrationData CalibrationData { get; private set; }

        public async Task Initialize()
        {
            if (true == isInitalized)
            {
                throw new InvalidOperationException(string.Format("BMP180@{0:X}: Device already initalized.", this.I2cAddress));
            }

            Debug.WriteLine("BMP180@{0:X}: Inititalize.", this.I2cAddress);

            try
            {
                //Instantiate the I2CConnectionSettings using the device address
                I2cConnectionSettings settings = new I2cConnectionSettings(this.I2cAddress);
                //Set the I2C bus speed of connection to fast mode
                settings.BusSpeed = I2cBusSpeed.FastMode;
                //Use the I2CBus device selector to create an advanced query syntax string
                string advancedQuerySyntax = I2cDevice.GetDeviceSelector(BMP180Device.I2CControllerName);
                //Use the Windows.Devices.Enumeration.DeviceInformation class to create a collection using the advanced query syntax string
                DeviceInformationCollection deviceInfoCollection = await DeviceInformation.FindAllAsync(advancedQuerySyntax);
                //Instantiate the the I2C device using the device id of the I2CBus and the I2CConnectionSettings
                this.bmp180I2c = await I2cDevice.FromIdAsync(deviceInfoCollection[0].Id, settings);

                //Check if device was found
                if (bmp180I2c == null)
                {
                    Debug.WriteLine("BMP180@{0:X}: Device not found.", this.I2cAddress);
                }
                else
                {
                    isInitalized = true;
                    this.ReadCalibrationData();
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine("Exception: " + e.Message + "\n" + e.StackTrace);
                throw;
            }
        }

        public async Task<byte[]> ReadUncompestatedTemperature()
        {
            this.CheckState();

            Debug.WriteLine("BMP180@{0:X} - Read Uncompensated Temperature {1:X}.", this.I2cAddress, BMP180Commands.ReadTemperature);

            byte[] command = { (byte)BMP180Registers.Command, (byte)BMP180Commands.ReadTemperature };
            this.bmp180I2c.Write(command);
            await Task.Delay(5);
            return WriteRead(BMP180Registers.Result, 2);
        }

        public async Task<byte[]> ReadUncompestatedPressure(BMP180AccuracyMode ossMode)
        {
            this.CheckState();            

            byte pressureCommand = 0;
            int delay = 5;

            switch (ossMode)
            {
                case BMP180AccuracyMode.UltraLowPower:
                    pressureCommand = (byte)BMP180Commands.ReadPressureLowAccuracy;
                    delay = 5;
                    break;
                case BMP180AccuracyMode.Standard:
                    pressureCommand = (byte)BMP180Commands.ReadPressureStandardAccuracy;
                    delay = 8;
                    break;
                case BMP180AccuracyMode.HighResolution:
                    pressureCommand = (byte)BMP180Commands.ReadPressureHighAccuracy;
                    delay = 14;
                    break;
                case BMP180AccuracyMode.UltraHighResolution:
                    pressureCommand = (byte)BMP180Commands.ReadPressureUltraHighAccuracy;
                    delay = 26;
                    break;
            }

            Debug.WriteLine("BMP180@{0:X} - Read Uncompensated Pressure {1:X} with mode {2:X}.", this.I2cAddress, pressureCommand, ossMode);

            byte[] command = { (byte)BMP180Registers.Command, pressureCommand };
            this.bmp180I2c.Write(command);

            await Task.Delay(delay);

            return WriteRead(BMP180Registers.Result, 3);
        }

        public byte[] WriteRead(BMP180Registers reg, int size)
        {
            this.CheckState();

            Debug.WriteLine("BMP180@{0:X} - WriteRead({1}, {2}).", this.I2cAddress, reg, size);

            byte[] buffer = new byte[size];
            this.bmp180I2c.WriteRead(new[] { (byte)reg }, buffer);

            return buffer;
        }

        public async Task<BMP180SensorData> ReadSensorDataAsync(BMP180AccuracyMode oss)
        {
            this.CheckState();

            Debug.WriteLine("BMP180@{0:X} - Read Sensor Data", this.I2cAddress);

            // Read the Uncompestated values from the sensor.
            byte[] tData = await ReadUncompestatedTemperature();
            byte[] pData = await ReadUncompestatedPressure(oss);

            int ut = (tData[0] << 8) + tData[1];
            double up = (pData[0] * 256.0) + pData[1] + (pData[2] / 256.0);

            // Calculate real values
            int b5 = calculateB5(ut);

            int t = (b5 + 8) >> 4;
            //double temperature = t / 10.0;
            double temperature = (double)t / 10.0;

            double s = temperature - 25.0;
            double x = (x2 * Math.Pow(s, 2)) + (x1 * s) + x0;
            double y = (y2 * Math.Pow(s, 2)) + (y1 * s) + y0;
            double z = (up - x) / y;

            double pressure = (p2 * Math.Pow(z, 2)) + (p1 * z) + p0;

            return new BMP180SensorData(temperature, pressure, tData, pData, oss);
        }

        private int calculateB5(int ut)
        {
            int X1 = ((ut - this.CalibrationData.AC6) * (this.CalibrationData.AC5)) >> 15;
            int X2 = (this.CalibrationData.MC << 11) / (X1 + this.CalibrationData.MD);
            return X1 + X2;
        }

        private void CheckState()
        {
            if (false == this.isInitalized)
            {
                throw new InvalidOperationException(string.Format("BMP180@{0:X} - Device not initialized.", this.I2cAddress));
            }

            if (true == this.disposedValue)
            {
                throw new InvalidOperationException(string.Format("BMP180@{0:X} - Device disposed.", this.I2cAddress));
            }
        }

        private void ReadCalibrationData()
        {
            byte[] data = WriteRead(BMP180Registers.AC1, 2);
            Array.Reverse(data);
            this.CalibrationData.AC1 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.AC2, 2);
            Array.Reverse(data);
            this.CalibrationData.AC2 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.AC3, 2);
            Array.Reverse(data);
            this.CalibrationData.AC3 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.AC4, 2);
            Array.Reverse(data);
            this.CalibrationData.AC4 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180Registers.AC5, 2);
            Array.Reverse(data);
            this.CalibrationData.AC5 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180Registers.AC6, 2);
            Array.Reverse(data);
            this.CalibrationData.AC6 = BitConverter.ToUInt16(data, 0);

            data = WriteRead(BMP180Registers.B1, 2);
            Array.Reverse(data);
            this.CalibrationData.B1 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.B2, 2);
            Array.Reverse(data);
            this.CalibrationData.B2 = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.MB, 2);
            Array.Reverse(data);
            this.CalibrationData.MB = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.MC, 2);
            Array.Reverse(data);
            this.CalibrationData.MC = BitConverter.ToInt16(data, 0);

            data = WriteRead(BMP180Registers.MD, 2);
            Array.Reverse(data);
            this.CalibrationData.MD = BitConverter.ToInt16(data, 0);


            // Compute floating-point polynominals
            double c3 = 160.0 * Math.Pow(2, -15) * this.CalibrationData.AC3;
            double c4 = Math.Pow(10, -3) * Math.Pow(2, -15) * this.CalibrationData.AC4;
            double b1 = Math.Pow(160, 2) * Math.Pow(2, -30) * this.CalibrationData.B1;
            double c5 = (Math.Pow(2, -15) / 160) * this.CalibrationData.AC5;
            double c6 = this.CalibrationData.AC6;
            double mc = (Math.Pow(2, 11) / Math.Pow(160, 2)) * this.CalibrationData.MC;
            double md = this.CalibrationData.MD / 160.0;
            this.x0 = this.CalibrationData.AC1;
            this.x1 = 160.0 * Math.Pow(2, -13) * this.CalibrationData.AC2;
            this.x2 = Math.Pow(160, 2) * Math.Pow(2, -25) * this.CalibrationData.B2;
            this.y0 = c4 * Math.Pow(2, 15);
            this.y1 = c4 * c3;
            this.y2 = c4 * b1;
            this.p0 = (3791.0 - 8.0) / 1600.0;
            this.p1 = 1.0 - 7357.0 * Math.Pow(2, -20);
            this.p2 = 3038.0 * 100.0 * Math.Pow(2, -36);
        }

        private void DeinitalizeDevice()
        {
            Debug.WriteLine("BMP180@{0:X} Deinitialize.", this.I2cAddress);
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    this.DeinitalizeDevice();
                    this.bmp180I2c.Dispose();
                }              

                disposedValue = true;
            }
        }

        ~BMP180Device() {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
