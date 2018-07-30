#region Header
// IBarcode.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 2:41 PM
#endregion

namespace ThermalTalk
{
    public interface IBarcode
    {
        /// <summary>
        /// String to encode
        /// </summary>
        string EncodeThis { get; set; }

        /// <summary>
        /// Barcode form can be form 1 or form 2
        /// Default: Form 1
        /// </summary>
        byte Form { get; set; }

        /// <summary>
        /// Barcode height parameter in dots. 1 dot
        /// is 1/8 mm
        /// </summary>
        byte BarcodeDotHeight { get; set; }

        /// <summary>
        /// Barcode width multiplier
        /// This multiplies the entire width of the barcode.
        /// An unscaled barcode at it thinnest is 1 dot wide (1/8mm)
        /// Code128 with a scalar of 1 may not be readable by some scalars
        /// </summary>
        byte BarcodeWidthMultiplier { get; set; }

        /// <summary>
        /// Where to place the HRI string
        /// </summary>
        HRIPositions HriPosition { get; set; }

        /// <summary>
        /// Which font to utilize for HRI font. Only options A
        /// and B can be used.
        /// </summary>
        ThermalFonts BarcodeFont { get; set; }

        /// <summary>
        /// Builds out the barcode payload. If there are any issues,
        /// an empty payload will be returned.
        /// </summary>
        /// <returns>byte[] payload or empty on error</returns>
        byte[] Build();
    }
}