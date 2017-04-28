#region Copyright & License
/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion
namespace ThermalTalk
{
    /// <summary>
    /// Collection of all possible printer statuses. 
    /// </summary>
    public sealed class PhoenixStatus : IStatus
    {
        public PhoenixStatus() { }

        /// <summary>
        /// Printer is reporting online if value is true
        /// </summary>
        public IsOnlineVal IsOnline { get; set; }

        /// <summary>
        /// There is some paper present if this value is true. Note, the paper level 
        /// may be low but is still conidered present.
        /// </summary>
        public IsPaperPresentVal IsPaperPresent { get; set; }

        /// <summary>
        /// Paper level is at or above the low paper threshold if value is true
        /// </summary>
        public IsPaperLevelOkayVal IsPaperLevelOkay { get; set; }

        /// <summary>
        /// Paper is in the present position if this value is true
        /// </summary>
        public IsTicketPresentAtOutputVal IsTicketPresentAtOutput { get; set; }

        /// <summary>
        /// Printer head (cover) is closed if value is true
        /// </summary>
        public IsCoverClosedVal IsCoverClosed { get; set; }

        /// <summary>
        /// The paper motor is currently off if this value is true
        /// </summary>
        public IsPaperMotorOffVal IsPaperMotorOff { get; set; }

        /// <summary>
        /// The diagnostic button is NOT being pushed if this value is true
        /// </summary>
        public IsDiagButtonReleasedVal IsDiagButtonReleased { get; set; }

        /// <summary>
        /// The head temperature is okay if this value is true
        /// </summary>
        public IsHeadTemperatureOkayVal IsHeadTemperatureOkay { get; set; }

        /// <summary>
        /// Comms are okay, no errors, if this value is true
        /// </summary>
        public IsCommsOkayVal IsCommsOkay { get; set; }

        /// <summary>
        /// Power supply voltage is within tolerance if this value is true
        /// </summary>
        public IsPowerSupplyVoltageOkayVal IsPowerSupplyVoltageOkay { get; set; }

        /// <summary>
        /// Power supply voltage is within tolerance if this value is true
        /// </summary>
        public IsPaperPathClearVal IsPaperPathClear { get; set; }

        /// <summary>
        /// The cutter is okay if this value is true
        /// </summary>
        public IsCutterOkayVal IsCutterOkay { get; set; }

        /// <summary>
        /// Last paper feed was NOT due to diag push button if value is true
        /// </summary>
        public IsNormalFeedVal IsNormalFeed { get; set; }

        /// <summary>
        /// If the printer is reporting any error type, this value is true
        /// </summary>
        public HasErrorVal HasError { get; set; }

        /// <summary>
        /// There is a non-recoverable error state if this value is true
        /// </summary>
        public HasFatalErrorVal HasFatalError { get; set; }

        /// <summary>
        /// There is a recoverable error state if this value is true
        /// </summary>
        public HasRecoverableErrorVal HasRecoverableError { get; set; }

        /// <summary>
        /// Returns this object as a JSON string and optionally
        /// tab-format so it looks pretty.
        /// </summary>
        /// <param name="prettyPrint">True to pretty print, default false</param>
        /// <returns>JSON string</returns>
        public string ToJSON(bool prettyPrint = false)
        {
            return Json.Serialize(this, prettyPrint);
        }
    }
}
