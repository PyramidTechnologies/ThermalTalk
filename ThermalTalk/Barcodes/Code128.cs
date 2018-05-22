#region Header
// Code128.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 2:24 PM
#endregion

namespace ThermalTalk
{
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Code 128 has extended modes each supporting a subset of characters.
    /// </summary>
    public class Code128 : BaseBarcode
    {
        /// <summary>
        /// Submodes of Code 128
        /// <see href="https://en.wikipedia.org/wiki/Code_128#Subtypes"/>
        /// </summary>
        public enum Modes
        {
            /// <summary>
            /// ASCII 0-95, special characters, and FNC 1-4
            /// </summary>
            A,
            /// <summary>
            /// ASCII 32-127, special characters, and FNC-1-4
            /// </summary>
            B,
            /// <summary>
            /// 00-99 encodes two digits with a single codepoint
            /// </summary>
            C
        }

        /// <summary>
        /// Gets or Sets encoding mode
        /// </summary>
        public Modes Mode { get; set; }
        
        /// <inheritdoc />
        /// <summary>
        /// Encodes as Code 128. Any characters not supported in your configuration
        /// will be omtted from the final barcode. If any illegal configuration is
        /// found, an empty payload will be returned.
        /// </summary>
        /// <returns>Payload or empty on error</returns>
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // If user explicitly asks for form 2, otherwise assume 1
            var m = (byte)(Form == 2 ? 73 : 8);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // mode 2 requires length
            if (m == 69)
            {
                payload.Add((byte)EncodeThis.Length);
            }

            // Add mode bytes
            var mode = Mode == Modes.A ? 0x41 : Mode == Modes.B ? 0x42 : 0x43;
            payload.AddRange(new byte[] {0x7B, (byte) mode });

            // Force null terminated string
            if (!EncodeThis.EndsWith("\0"))
            {
                payload.Add(0);
            }

            var bytes = System.Text.Encoding.ASCII.GetBytes(EncodeThis);
            payload.AddRange(bytes);

            return payload.ToArray();
        }
    }
}