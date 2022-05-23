using System;

namespace ThermalTalk
{
    /// <summary>
    /// Create a new barcode section
    /// </summary>
    public class BarcodeSection : ISection
    {
        private readonly IBarcode _barcode;

        /// <summary>
        /// Create a new section using the specified barcode
        /// </summary>
        /// <param name="barcode"></param>
        public BarcodeSection(IBarcode barcode)
        {
            _barcode = barcode;
        }

        /// <inheritdoc />
        public string Content { get; set; }

        /// <inheritdoc />
        public FontEffects Effects { get; set; }

        /// <inheritdoc />
        public FontJustification Justification { get; set; }

        /// <inheritdoc />
        public FontWidthScalar WidthScalar { get; set; }

        /// <inheritdoc />
        public FontHeighScalar HeightScalar { get; set; }

        /// <inheritdoc />
        public ThermalFonts Font { get; set; }

        /// <inheritdoc />
        public bool AutoNewline { get; set; }

        /// <inheritdoc />
        public BufferAction GetContentBuffer(CodePages codepage)
        {
            var delay = _barcode.BarcodeFlavor == TwoDBarcode.Flavor.Reliance
                ? TimeSpan.FromMilliseconds(500)
                : TimeSpan.Zero;
            
            return new BufferAction
            {
                Buffer = _barcode?.Build() ?? new byte[0],
                AfterSendDelay = delay
            };
        }
    }
}