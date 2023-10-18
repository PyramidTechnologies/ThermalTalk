#region Header
// Code39.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 2:14 PM
#endregion

namespace ThermalTalk
{
    using System.Collections.Generic;

    /// <inheritdoc />
    /// <summary>
    /// Code39 barcode implementation supports A-Z,a-z,0-9 and
    /// space,$,%,+,-,.,/,: (no comma)
    /// </summary>
    public class Code39 : BaseBarcode
    {
        /// <inheritdoc />
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // If user explicitly asks for form 2, otherwise assume 1
            var m = (byte)(Form == 2 ? 69 : 4);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // mode 2 requires length
            if (m == 69)
            {
                payload.Add((byte)EncodeThis.Length);
            }

            var bytes = System.Text.Encoding.ASCII.GetBytes(EncodeThis);
            payload.AddRange(bytes);


            // Force null terminated string
            if (EncodeThis[EncodeThis.Length - 1] != '\0')
            {
                payload.Add(0);
            }

            return payload.ToArray();
        }
    }
}