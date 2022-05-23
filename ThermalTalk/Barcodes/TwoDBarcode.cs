namespace ThermalTalk
{
    using System.Linq;
    using System.Text;

    /// <summary>
    /// General 2D barcode
    /// </summary>
    public class TwoDBarcode : IBarcode
    {
        /// <summary>
        /// 2D barcode flavor
        /// </summary>
        public enum Flavor
        {
            /// <summary>
            ///     Phoenix style
            /// </summary>
            Phoenix,

            /// <summary>
            ///     Reliance style
            /// </summary>
            Reliance
        }


        /// <summary>
        /// Create a new 2D barcode
        /// Note that Phoenix and Reliance 2D barcodes are slightly different
        /// so you must specify the flavor parameter.
        /// </summary>
        /// <param name="flavor">Printer flavor</param>
        public TwoDBarcode(Flavor flavor)
        {
            BarcodeFlavor = flavor;
        }

        public Flavor BarcodeFlavor { get; }

        /// <inheritdoc />
        public string EncodeThis { get; set; }

        /// <inheritdoc />
        public byte Form { get; set; }

        /// <inheritdoc />
        public byte BarcodeDotHeight { get; set; }

        /// <inheritdoc />
        public byte BarcodeWidthMultiplier { get; set; }

        /// <inheritdoc />
        public HRIPositions HriPosition { get; set; }

        /// <inheritdoc />
        public ThermalFonts BarcodeFont { get; set; }

        /// <inheritdoc />
        public byte[] Build()
        {
            switch (BarcodeFlavor)
            {
                case Flavor.Phoenix:
                    return BuildPhoenixFlavor();
                case Flavor.Reliance:
                    return BuildRelianceFlavor();
                default:
                    return new byte[0];
            }
        }

        /// <summary>
        /// Build 2D barcode using Phoenix syntax
        /// </summary>
        /// <returns>2D barcode generator command</returns>
        private byte[] BuildPhoenixFlavor()
        {
            var len = EncodeThis.Length > 32 ? 32 : EncodeThis.Length;
            var pL = len + 3;
            
            var setup = new byte[] 
            { 
                0x1D, 0x28, 0x6B, 
                (byte) pL, // pL
                0x00, // pH
                0x31, // cn 
                0x50, // fn
                0x31
            };
            var printIt = new byte[]
            {
                0x1D, 0x28, 0x6B, 
                0x03, // pL
                0x00, // pH
                0x31, // cn
                0x51, // fn
                0x31
            };

            var toEncode = EncodeThis.Take(len).ToArray();
            
            var fullCmd = Extensions.Concat(setup, Encoding.ASCII.GetBytes(toEncode), printIt);
            return fullCmd;
        }

        /// <summary>
        /// Build 2D barcode using Reliance syntax
        /// </summary>
        /// <returns>2D barcode generator command</returns>
        private byte[] BuildRelianceFlavor()
        {
            var len = EncodeThis.Length > 154 ? 154 : EncodeThis.Length;
            var setup = new byte[] { 0x0A, 0x1C, 0x7D, 0x25, (byte) len };

            var fullCmd = Extensions.Concat(setup, Encoding.ASCII.GetBytes(EncodeThis), new byte[] { 0x0A });
            return fullCmd;
        }
    }
}