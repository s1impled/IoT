namespace SSD1306.Fonts
{
    public sealed class FontCharacterDescriptor
    {
        public FontCharacterDescriptor(char Chr, uint CharHeightBytes, byte[] CharData)
        {
            Character = Chr;
            CharacterHeightBytes = CharHeightBytes;
            CharacterData = CharData;
        }

        public char Character { get; private set; }

        public byte[] CharacterData { get; private set; }

        public uint CharacterHeightBytes { get; private set; }

        public uint CharacterWidthPx { get { return (uint)CharacterData.Length / CharacterHeightBytes; } }
    }
}
