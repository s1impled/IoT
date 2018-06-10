namespace SSD1306.Images
{
    public sealed class DisplayImage
    {
        public DisplayImage(uint imageHeightBytes, byte[] imageData)
        {
            ImageHeightBytes = imageHeightBytes;
            ImageData = imageData;
        }

        public uint ImageWidthPx { get { return (uint)ImageData.Length / ImageHeightBytes; } }

        public uint ImageHeightBytes { get; private set; }

        public byte[] ImageData { get; private set; }
    }
}
