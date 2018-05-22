#region Header
// BaseBarcode.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 2:07 PM
#endregion

namespace ThermalTalk
{
    using System.Collections.Generic;

    /// <summary>
    /// Contract defines what a barcode generator should perform and provide
    /// </summary>
    public abstract class BaseBarcode : IBarcode
    {
        /// <inheritdoc />
        public string EncodeThis { get; set; }

        /// <inheritdoc />
        public byte Form { get; set; }

        /// <inheritdoc />
        public byte BarcodeDotHeight { get; set; } = 100;

        /// <inheritdoc />
        public byte BarcodeWidthMultiplier { get; set; } = 2;

        /// <inheritdoc />
        public HRIPositions HriPosition { get; set; }

        /// <inheritdoc />
        public ThermalFonts BarcodeFont { get; set; }

        /// <inheritdoc />
        public abstract byte[] Build();

        /// <summary>
        /// Build out barcode pre-config args, if any
        /// </summary>
        /// <returns>byte[] payload or empty if no config needed</returns>
        protected List<byte> Preamble()
        {
            var payload = new List<byte>();

            if (BarcodeDotHeight > 0)
            {
                payload.AddRange(new byte[] { 0x1D, 0x68, BarcodeDotHeight });
            }

            if (BarcodeWidthMultiplier >= 1 && BarcodeWidthMultiplier <=6)
            {
                payload.AddRange(new byte[] { 0x1D, 0x77, BarcodeDotHeight });
            }

            payload.AddRange(new byte[] { 0x1D, 0x48, (byte)HriPosition });

            payload.AddRange(new byte[] { 0x1D, 0x66, (byte)(BarcodeFont == ThermalFonts.A ? 0 : 1) });

            return payload;
        }
    }
}