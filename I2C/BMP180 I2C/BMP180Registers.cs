namespace BMP180.I2C
{
    /// <summary>
    /// Represents BML180 I2c Registers
    /// </summary>
    public enum BMP180Registers : byte
    {
        /// <summary>
        /// AC calibration register
        /// </summary>
        AC1 = 0xAA,

        /// <summary>
        /// AC2 calibration register
        /// </summary>
        AC2 = 0xAC,

        /// <summary>
        /// AC3 calibration register
        /// </summary>
        AC3 = 0xAE,

        /// <summary>
        /// AC4 calibration register
        /// </summary>
        AC4 = 0xB0,

        /// <summary>
        /// AC5 calibration register
        /// </summary>
        AC5 = 0xB2,

        /// <summary>
        /// AC6 calibration register
        /// </summary>
        AC6 = 0xB4,

        /// <summary>
        /// B1 calibration register
        /// </summary>
        B1 = 0xB6,

        /// <summary>
        /// B2 calibration register
        /// </summary>
        B2 = 0xB8,

        /// <summary>
        /// MB calibration register
        /// </summary>
        MB = 0xBA,

        /// <summary>
        /// MC calibration register
        /// </summary>
        MC = 0xBC,

        /// <summary>
        /// MD calibration register
        /// </summary>
        MD = 0xBE,

        /// <summary>
        /// I2c Comms test register. Returns 0x55 always
        /// </summary>
        ChipId = 0xD0,

        /// <summary>
        /// BMP180 Command register.
        /// </summary>
        Command = 0xF4,

        /// <summary>
        /// BMP180 Read Register
        /// </summary>
        Result = 0xF6
    }

    /// <summary>
    /// Represents BMP180 commands
    /// </summary>
    public enum BMP180Commands : byte
    {
        /// <summary>
        /// Command to read temperature data
        /// </summary>
        ReadTemperature = 0x2E,

        /// <summary>
        /// Command to read pressure data with low accuracy
        /// </summary>
        ReadPressureLowAccuracy = 0x34,

        /// <summary>
        /// Command to read pressure data with standard accuracy
        /// </summary>
        ReadPressureStandardAccuracy = 0x74,

        /// <summary>
        /// Command to read pressure data with high accuracy
        /// </summary>
        ReadPressureHighAccuracy = 0xB4,

        /// <summary>
        /// Command to read pressure data with ultra-high accuracy
        /// </summary>
        ReadPressureUltraHighAccuracy = 0xF4,

        /// <summary>
        /// Command to soft reset sensor
        /// </summary>
        SoftReset = 0xE0
    }
}
