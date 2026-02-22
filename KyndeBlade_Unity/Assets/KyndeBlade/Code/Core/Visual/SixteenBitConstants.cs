namespace KyndeBlade
{
    /// <summary>SNES-style 16-bit pipeline constants. Use for resolution, palette limits, or UI reference resolution when 16-bit mode is active.</summary>
    public static class SixteenBitConstants
    {
        /// <summary>Typical native width (SNES).</summary>
        public const int NativeWidth = 256;
        /// <summary>Typical native height (SNES).</summary>
        public const int NativeHeight = 224;
        /// <summary>HD upscale width (2×).</summary>
        public const int HdWidth = 512;
        /// <summary>HD upscale height (2×).</summary>
        public const int HdHeight = 448;
        /// <summary>Max colors on screen (SNES spec).</summary>
        public const int MaxColorsOnScreen = 256;
    }
}
