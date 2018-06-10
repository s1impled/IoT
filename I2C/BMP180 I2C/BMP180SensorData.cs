using System;
using System.Text;

namespace BMP180.I2C
{
    /// <summary>
    /// Represents sensor data from the BMP180 sensor
    /// </summary>
    public sealed class BMP180SensorData
    {
        /// <summary>
        /// Pressure at Sea Level for calculating altitude from pressure.
        /// </summary>
        private double pressureAtSeaLevel;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Temperature"></param>
        /// <param name="Pressure"></param>
        /// <param name="UncompensatedTemperature"></param>
        /// <param name="UncompensatedPressure"></param>
        public BMP180SensorData(double temperature, double pressure, byte[] uncompensatedTemperature, byte[] uncompensatedPressure, BMP180AccuracyMode mode)
        {
            // Remove 11 degrees for heat offset from RPI.
            // Probably more after case is installed.
            this.Temperature = temperature - 11.0;
            this.Pressure = pressure;
            this.UncompensatedTemperature = uncompensatedTemperature;
            this.UncompensatedPressure = uncompensatedPressure;
            this.AccuracyMode = mode;
            this.PressureAtSeaLevelHpa = 1013.25;
        }

        /// <summary>
        /// Gets the altitude in meters from this sensor reading
        /// </summary>
        public double Altitude { get; private set; }

        /// <summary>
        /// Gets the altitude in feet from this sensor reading
        /// </summary>
        public double AltitudeInFeet
        {
            get
            {
                return this.Altitude * 3.2808399;
            }
        }

        /// <summary>
        /// Gets or sets the current pressure at sea level in hPa
        /// </summary>
        public double PressureAtSeaLevelHpa {
            get
            {
                return this.pressureAtSeaLevel;
            }

            set
            {
                this.pressureAtSeaLevel = value;
                this.Altitude = 44330.0 * (1.0 - Math.Pow(this.Pressure / this.pressureAtSeaLevel, 0.1903));
            }
        }

        /// <summary>
        /// Gets the temperature reading
        /// </summary>
        public double Temperature { get; private set; }

        /// <summary>
        /// Get the temperature reading convered to Fahrenheit
        /// </summary>
        public double TemperatureInFahrenheit
        {
            get {
                return this.Temperature * 1.8 + 32;
            }
        }

        /// <summary>
        /// Gets the pressure reading
        /// </summary>
        public double Pressure { get; private set; }

        /// <summary>
        /// Gets the raw temperature reading
        /// </summary>
        public byte[] UncompensatedTemperature { get; private set; }

        /// <summary>
        /// Gets the raw presure reading
        /// </summary>
        public byte[] UncompensatedPressure { get; private set; }

        /// <summary>
        /// Gets the accuracy mode for the reading
        /// </summary>
        public BMP180AccuracyMode AccuracyMode { get; private set; }

        /// <summary>
        /// Returns a string representation of this object
        /// </summary>
        /// <returns>a string</returns>
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("Temperature {0:N2}\u00b0 C\r\n", this.Temperature);
            sb.AppendFormat("Temperature {0:N2}\u00b0 F\r\n", this.TemperatureInFahrenheit);
            sb.AppendFormat("Pressure {0:N2} Pa\r\n", this.Pressure);           
            sb.AppendFormat("Altitude {0:N2} m\r\n", this.Altitude);
            sb.AppendFormat("Altitude {0:N1} \'\r\n", this.AltitudeInFeet);
            sb.AppendFormat("AccuracyMode {0}\r\n", this.AccuracyMode);
            sb.AppendFormat("Uncompensated Temperature [{0}]\r\n", BitConverter.ToString(this.UncompensatedTemperature));
            sb.AppendFormat("Uncompensated Pressure [{0}]\r\n", BitConverter.ToString(this.UncompensatedPressure));

            return sb.ToString();
        }
    }
}
