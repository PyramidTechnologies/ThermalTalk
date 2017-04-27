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
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Reliance Printer is the primary handle for accessing the printer API
    /// </summary>
    public class ReliancePrinter : BasePrinter
    {
        const int DefaultReadTimeout = 1000; /// ms
        const int DefaultBaudRate = 19200;

        /// <summary>
        /// Constructs a new instance of ReliancePrinter. This printer
        /// acts as a handle to all features and functions. If the serial port parameter
        /// is provided, the serial connection will be opened immediately.
        /// </summary>
        /// <param name="serialPortName">OS name of serial port</param>        
        public ReliancePrinter(string serialPortName)
        {

            EnableCommands = new Dictionary<FontEffects, byte[]>()
            {
                { FontEffects.None, new byte[0]},
                { FontEffects.Bold, new byte[] { 0x1B, 0x45, 0x1 }},
                { FontEffects.Italic, new byte[] { 0x1B, 0x34, 0x1 }},
                { FontEffects.Underline, new byte[] { 0x1B, 0x2D, 0x1 }},
                { FontEffects.Rotated, new byte[] { 0x1B, 0x56, 0x1 }},
                { FontEffects.Reversed, new byte[] { 0x1D, 0x42, 0x1 }},
                { FontEffects.UpsideDown, new byte[] { 0x1B, 0x7B, 0x1 }},
            };

            DisableCommands = new Dictionary<FontEffects, byte[]>()
            {
                { FontEffects.None, new byte[0]},
                { FontEffects.Bold, new byte[] { 0x1B, 0x45, 0x0 }},
                { FontEffects.Italic, new byte[] { 0x1B, 0x34, 0x0 }},
                { FontEffects.Underline, new byte[] { 0x1B, 0x2D, 0x0 }},
                { FontEffects.Rotated, new byte[] { 0x1B, 0x56, 0x0 }},
                { FontEffects.Reversed, new byte[] { 0x1D, 0x42, 0x0 }},
                { FontEffects.UpsideDown, new byte[] { 0x1B, 0x7B, 0x0 }},
            };

            JustificationCommands = new Dictionary<FontJustification, byte[]>()
            {           
                { FontJustification.JustifyLeft, new byte[0]},
                { FontJustification.JustifyLeft, new byte[] { 0x1B, 0x61, 0x00 }},
                { FontJustification.JustifyCenter, new byte[] { 0x1B, 0x61, 0x01 }},
                { FontJustification.JustifyRight, new byte[] { 0x1B, 0x61, 0x02 }},
            };

            SetScalarCommand = new byte[] { 0x1D, 0x21, 0x00};  // last byte set by tx func
            FormFeedCommand = new byte[] { 0x0C };
            NewLineCommand = new byte[] { 0x0A };
            InitPrinterCommand = new byte[] { 0x1B, 0x40 };

            // User wants a serial port
            if (!string.IsNullOrEmpty(serialPortName))
            {            
                PrintSerialReadTimeout = DefaultReadTimeout;
                PrintSerialBaudRate = DefaultBaudRate;

                Connection = new RelianceSerialPort(serialPortName, PrintSerialBaudRate);
                Connection.ReadTimeoutMS = DefaultReadTimeout;              
            }
        }

        /// <summary>
        /// Encodes the specified string as a center justified 2D barcode. 
        /// This 2D barcode is compliant with the QR Code® specicification and can be read by all 2D barcode readers.
        /// Up to 154 8-bit characters are supported.
        /// f the input string length exceeds the range specified by the k parameter, only the first 154 characters will be 
        /// encoded. The rest of the characters to be encoded will be printed as regular ESC/POS characters on a new line.
        /// </summary>
        /// <param name="encodeThis">String to encode, max length = 154 bytes</param>
        public void Print2DBarcode(string encodeThis)
        {
            var len = encodeThis.Length > 154 ? 154 : encodeThis.Length;
            var setup = new byte[] { 0x0A, 0x1C, 0x7D, 0x25, (byte)len };

            var fullCmd = Extensions.Concat(setup, ASCIIEncoding.ASCII.GetBytes(encodeThis), new byte[] { 0x0A });

        }

        /// <summary>
        /// This command is processed in real time. The reply to this command is sent
        /// whenever it is received and does not wait for previous ESC/POS commands to be executed first.
        /// If there is no response or an invalid response, all fields of RealTimeStatus will be null.
        /// </summary>
        /// <param name="r">StatusRequest type</param>
        /// <returns>Instance of RelianceStatus,m null on failure, Unset fields will be null</returns>
        public RelianceStatus GetStatus(RelianceStatusRequests r)
        {
            // Result stored here
            RelianceStatus rts = null;

            // Send the real time status command, r is the argument
            var command = new byte[] { 0x10, 0x04, (byte)r };
            int respLen = (r == RelianceStatusRequests.FullStatus) ? 6 : 1;

            var data = new byte[0];

            try
            {
                Connection.Open();

                var written = Connection.Write(command);

                System.Threading.Thread.Sleep(250);

                // Collect the response
                data = Connection.Read(respLen);

            }
            catch
            { }
            finally
            {
                Connection.Close();
            }
            
            // Invalid response
            if(data.Length != respLen)
            {
                return rts;
            }

            rts = new RelianceStatus();

            switch(r)
            {
                case RelianceStatusRequests.Status:
                    // bit 3: 0- online, 1- offline        
                    rts.IsOnline = (data[0] & 0x08) == 0;
                    break;

                case RelianceStatusRequests.OffLineStatus:
                    // bit 2: 0- no error, 1- error        
                    rts.IsCoverClosed = (data[0] & 0x04) == 0;

                    // bit 3: 0- no error, 1- error        
                    rts.IsNormalFeed = (data[0] & 0x08) == 0;

                    // bit 5: 0- no error, 1- error        
                    rts.IsPaperPresent = (data[0] & 0x20) == 0;

                    // bit 6: 0- no error, 1- error        
                    rts.HasError = (data[0] & 0x40) == 0;
                 
                    break;

                case RelianceStatusRequests.ErrorStatus:
                    // bit 3: 0- okay, 1- Not okay    
                    rts.IsCutterOkay = (data[0] & 8) == 0;    

                    // bit 5: 0- No fatal (non-recoverable) error, 1- Fatal error        
                    rts.HasFatalError = (data[0] & 8) == 0;    
    
                    // bit 6: 0- No recoverable error, 1- Recoverable error        
                    rts.HasRecoverableError = (data[0] & 0x40) == 1; 
                    break;

                case RelianceStatusRequests.PaperRollStatus:
                    /// bit 2,3: 0- okay, 12- Not okay        
                    rts.IsPaperLevelOkay = (data[0] & 0x0C) == 0;

                    /// bit 5,6: 0- okay, 96- Not okay
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;                    
                    break;

                case RelianceStatusRequests.PrintStatus:
                    /// bit 2: 0- motor off, 1: motor on        
                    rts.IsPaperMotorOff = (data[0] & 4) == 0;
                    // bit 5: 0- paper present, 1: motor stopped because out of paper
                    
                    rts.IsPaperPresent = (data[0] & 4) == 0;
                    break;

                case RelianceStatusRequests.FullStatus:

                    rts.IsPaperPresent = (data[2] & 0x01) == 0;
                    rts.IsPaperLevelOkay = (data[2] & 0x04) == 0;
                    rts.IsTicketPresentAtOutput = (data[2] & 0x20) == 0;

                    // Custom specs duplicates these so if EITHER
                    // are set to open, report open
                    var covera = (data[3] & 0x01) == 0;
                    var coverb = (data[3] & 0x02) == 0;

                    rts.IsCoverClosed = covera && coverb;

                    rts.IsPaperMotorOff = (data[3] & 0x08) == 0;
                    rts.IsDiagButtonReleased = (data[3] & 0x20) == 0;

                    rts.IsHeadTemperatureOkay = (data[4] & 0x01) == 0;
                    rts.IsCommsOkay = (data[4] & 02) == 0;
                    rts.IsPowerSupplyVoltageOkay = (data[4] & 0x08) == 0;
                    rts.IsPaperPathClear = (data[4] & 0x40) == 0;

                    rts.IsCutterOkay = (data[5] & 0x01) == 0; 
                    break;
                    
            }

            return rts;
        }
    }
}
