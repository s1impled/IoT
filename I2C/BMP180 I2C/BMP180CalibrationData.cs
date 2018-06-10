namespace BMP180.I2C
{
    /// <summary>
    /// Represents Calibration data from BMP180 sensor
    /// </summary>
    public class BMP180CalibrationData
    {
        public short AC1 { get; set; }
        public short AC2 { get; set; }
        public short AC3 { get; set; }
        public ushort AC4 { get; set; }
        public ushort AC5 { get; set; }
        public ushort AC6 { get; set; }
        public short B1 { get; set; }
        public short B2 { get; set; }
        public short MB { get; set; }
        public short MC { get; set; }
        public short MD { get; set; }

        public override string ToString()
        {
            return "{ AC1: " + AC1.ToString("X") +
                   ", AC2: " + AC2.ToString("X") +
                   ", AC3: " + AC3.ToString("X") +
                   ", AC4: " + AC4.ToString("X") +
                   ", AC5: " + AC5.ToString("X") +
                   ", AC6: " + AC6.ToString("X") +
                   ", VB1: " + B1.ToString("X") +
                   ", VB2: " + B2.ToString("X") +
                   ", MB: " + MB.ToString("X") +
                   ", MC: " + MC.ToString("X") +
                   ", MD: " + MD.ToString("X") + " }";
        }
    }
}
