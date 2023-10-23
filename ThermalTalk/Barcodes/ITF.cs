#region Header
// ITF.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 2:24 PM
#endregion

namespace ThermalTalk
{
    using System.Linq;

    /// <inheritdoc />
    /// <summary>
    /// ITF is a number only barcode format
    /// </summary>
    public class ITF : BaseBarcode
    {
        /// <inheritdoc />
        public override byte[] Build()
        {
            if (string.IsNullOrEmpty(EncodeThis))
            {
                return new byte[0];
            }

            // Must be even length
            if (EncodeThis.Length % 2 != 0)
            {
                return new byte[0];
            }

            // Must be numeric
            if (EncodeThis.ToCharArray().Any(ch => !char.IsDigit(ch)))
            {
                return new byte[0];
            }

            // If user explicitly asks for form 2, otherwise assume 1
            var m = (byte)(Form == 2 ? 70 : 5);

            var payload = Preamble();
            payload.AddRange(new byte[] { 0x1D, 0x6B, m });

            // mode 2 requires length
            if (m == 70)
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