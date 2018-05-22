#region Header
// BarcodeTypes.cs
// ThermalTalk
// Cory Todd
// 22-05-2018
// 12:50 PM
#endregion

namespace ThermalTalk
{
    public enum BarcodeTypes
    {
        /// <summary>
        /// Supported since 1.18 firmware
        /// </summary>
        Code39,
        /// <summary>
        /// Supported since 1.18 firmware
        /// </summary>
        Code128,
        /// <summary>
        /// Supported since 1.21 firmware
        /// </summary>
        ITF,
    }
}