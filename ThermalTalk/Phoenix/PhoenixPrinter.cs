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
    using ThermalTalk.Imaging;
    using System;
    using System.Text;

    /// <inheritdoc />
    public class PhoenixPrinter : BasePrinter
    {
               
        const int DefaultReadTimeout = 1500; /// ms
        const int DefaultBaudRate = 9600;

        private readonly byte[] FontACmd = { 0x1B, 0x50 };
        private readonly byte[] FontBCmd = { 0x1B, 0x54 };
        private readonly byte[] FontCCmd = { 0x1B, 0x55 };

        /// <inheritdoc />
        /// <summary>
        /// Constructs a new instance of PhoenixPrinter. This printer
        /// acts as a handle to all features and functions. If the serial port parameter
        /// is provided, the serial connection will be opened immediately.
        /// </summary>
        /// <param name="serialPortName">OS name of serial port</param>        
        public PhoenixPrinter(string serialPortName)
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
                { FontJustification.NOP, new byte[0]},
                { FontJustification.JustifyLeft, new byte[] { 0x1B, 0x61, 0x00 }},
                { FontJustification.JustifyCenter, new byte[] { 0x1B, 0x61, 0x01 }},
                { FontJustification.JustifyRight, new byte[] { 0x1B, 0x61, 0x02 }},
            };

            SetScalarCommand = new byte[] { 0x1D, 0x21, 0x00};  // last byte set by tx func
            FormFeedCommand = new byte[] { 0x1B, 0x64, 0x14, 0x1B, 0x6D };
            NewLineCommand = new byte[] { 0x0A };
            InitPrinterCommand = new byte[] { 0x1B, 0x40 };

            PrintSerialReadTimeout = DefaultReadTimeout;
            PrintSerialBaudRate = DefaultBaudRate;

            // User wants a serial port
            if (string.IsNullOrEmpty(serialPortName))
            {
                return;
            }

            Connection = new PhoenixSerialPort(serialPortName, PrintSerialBaudRate)
            {
                ReadTimeoutMS = DefaultReadTimeout
            };
        }

        /// <summary>
        /// Updates the formfeed line count to n.
        /// where 0 lt n lt 200
        /// Units are in lines relative to current font size. The default
        /// value is 20.
        /// </summary>
        /// <param name="n">Count of lines to print before cut</param>
        public void SetFormFeedLineCount(byte n)
        {
            Logger?.Trace("Setting form feed line count to: " + n);
            
            FormFeedCommand[2] = n;
        }

        /// <inheritdoc />
        /// <summary>
        /// Encodes the specified string as a center justified 2D barcode. 
        /// This 2D barcode is compliant with the QR Code® specicification and can be read by all 2D barcode readers.
        /// Up to 154 8-bit characters are supported.
        /// f the input string length exceeds the range specified by the k parameter, only the first 154 characters will be 
        /// encoded. The rest of the characters to be encoded will be printed as regular ESC/POS characters on a new line.
        /// </summary>
        /// <param name="encodeThis">String to encode, max length = 154 bytes</param>
        public override ReturnCode Print2DBarcode(string encodeThis)
        {
            Logger?.Trace("Encoding the following string as a barcode: " + encodeThis);

            // Use all default values for barcode
            var barcode = new TwoDBarcode(TwoDBarcode.Flavor.Phoenix)
            {
                EncodeThis = encodeThis
            };
            var fullCmd = barcode.Build();
            return AppendToDocBuffer(fullCmd);
        }

        /// <summary>
        /// Sets the active font to this
        /// </summary>
        /// <param name="font">Font to use</param>
        /// <returns>ReturnCode.Success if successful, ReturnCode.ExecutionFailure otherwise.</returns>
        public override ReturnCode SetFont(ThermalFonts font)
        {
            Logger?.Trace("Setting thermal fonts . . .");
            
            if (font == ThermalFonts.NOP)
            {
                Logger?.Trace("No change selected");
                
                return ReturnCode.Success;
            }

            var result = ReturnCode.ExecutionFailure;
            
            switch (font)
            {
                case ThermalFonts.A:
                    Logger?.Trace("Attempting to set font to font A.");
                    result =  AppendToDocBuffer(FontACmd);
                    break;
                case ThermalFonts.B:
                    Logger?.Trace("Attempting to set font to font B.");
                    result = AppendToDocBuffer(FontBCmd);
                    break;
                case ThermalFonts.C:
                    Logger?.Trace("Attempting to set font to font C.");
                    result =  AppendToDocBuffer(FontCCmd);
                    break;
            }

            return result;
        }

        /// <inheridoc/>
        public override ReturnCode SetImage(PrinterImage image, IDocument doc, int index)
        {
            
            while(index >= doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new PhoenixImageSection() {
                Image = image,
            };

            return ReturnCode.Success;
        }

        /// <summary>
        /// Phoenix support normal and double scalars. All other scalar values will
        /// be ignored.
        /// </summary>
        /// <param name="w">New scalar (1x, 2x, nop)</param>
        /// <param name="h">New scalar (1x, 2x, nop)</param>
        /// /// <returns>ReturnCode.Success if successful, ReturnCode.ExecutionFailure otherwise.</returns>
        public override ReturnCode SetScalars(FontWidthScalar w, FontHeighScalar h)
        {           
            var newWidth = Width;
            if(w == FontWidthScalar.NOP || w == FontWidthScalar.w1 || w == FontWidthScalar.w2)
            {
                newWidth = w;
            }

            var newHeight = Height;
            if (h == FontHeighScalar.NOP || h == FontHeighScalar.h1 || h == FontHeighScalar.h2)
            {
                newHeight = h;
            }

            // Only apply update if either property has changed
            if (newWidth != Width || newHeight != Height)
            {
                return base.SetScalars(newWidth, newHeight);
            }

            return ReturnCode.Success;
        }

        /// <inheritdoc />
        /// <summary>
        /// This command is processed in real time. The reply to this command is sent
        /// whenever it is received and does not wait for previous ESC/POS commands to be executed first.
        /// If there is no response or an invalid response, IsValidReport will be set to false
        /// </summary>
        /// <remarks>Phoenix does not support Error or Movement status request type</remarks>
        /// <param name="type">StatusRequest type</param>
        /// <returns>Instance of PhoenixStatus</returns>
        public override StatusReport GetStatus(StatusTypes type)
        {         
            ReturnCode ret;

            // Translate generic status to phoenix status
            PhoenixStatusRequests r;
            switch(type)
            {
                case StatusTypes.PrinterStatus:
                    r = PhoenixStatusRequests.Status;
                    break;

                case StatusTypes.OfflineStatus:
                    r = PhoenixStatusRequests.OffLineStatus;
                    break;

                case StatusTypes.ErrorStatus:
                    return StatusReport.Invalid();;
                    break;

                case StatusTypes.PaperStatus:
                    r = PhoenixStatusRequests.PaperRollStatus;
                    break;

                case StatusTypes.MovementStatus:
                    // Not supported on Phoenix
                    return StatusReport.Invalid();;

                case StatusTypes.FullStatus:
                    r = PhoenixStatusRequests.FullStatus;
                    break;

                default:
                    // Unknown status type
                    return StatusReport.Invalid();
            }

            var rts = new StatusReport();

            if (r == PhoenixStatusRequests.FullStatus)
            {
                ret = internalGetStatus(PhoenixStatusRequests.Status, rts);
                ret = ret != ReturnCode.Success ? ret : internalGetStatus(PhoenixStatusRequests.PaperRollStatus, rts);
                ret = ret != ReturnCode.Success ? ret : internalGetStatus(PhoenixStatusRequests.OffLineStatus, rts);

                // Not supported PP-82
                //ret = ret != ReturnCode.Success ? ret : internalGetStatus(PhoenixStatusRequests.ErrorStatus, rts);
            }
            else
            {
                ret = internalGetStatus(r, rts);
            }

            // Return null status object on error
            return ret == ReturnCode.Success ? rts : StatusReport.Invalid();
        }

        /// <summary>
        /// Write specified report type to target and fill rts with parsed response
        /// </summary>
        /// <param name="r">Report type, Phoenix does not support the error status command</param>
        /// <param name="rts">Target</param>
        /// <returns>ReturnCode.Success if successful, and ReturnCode.ExecutionFailure if there
        /// is an issue with the response received. Returns ReturnCode.Unsupported command
        /// if r == PhoenixStatusRequests.ErrorStatus. </returns>
        private ReturnCode internalGetStatus(PhoenixStatusRequests r, StatusReport rts)
        {
            
            Logger?.Trace("Attempting to get status of printer . . .");

            // PP-82 : Phoenix does not support the error status command
            if (r == PhoenixStatusRequests.ErrorStatus)
            {
                Logger?.Warn("PhoenixStatusRequests.ErrorStatus is unsupported . . .");
                return ReturnCode.UnsupportedCommand;
            }

            // Send the real time status command, r is the argument
            var command = new byte[] { 0x10, 0x04, (byte)r };
            int respLen = 1;

            var data = new byte[0];

            try
            {
                Logger?.Trace("Attempting to open connection . .. ");
                Connection.Open();

                var written = Connection.Write(command);

                System.Threading.Thread.Sleep(250);
                
                // Collect the response
                data = Connection.Read(respLen);

            }
            catch(Exception e)
            {
                Logger?.Error("The following exception was thrown while attempting to write the status:");
                Logger?.Error(e.Message);
                Logger?.Error(e.StackTrace);

                return ReturnCode.ExecutionFailure;

            }
            finally
            {
                
                Connection.Close();
            }

            // Invalid response
            if (data.Length != respLen)
            {
                Logger?.Trace("Data received is the incorrect length, returning execution failure . . . ");
                return ReturnCode.ExecutionFailure;
            }

            switch (r)
            {
                case PhoenixStatusRequests.Status:
                    // bit 3: 0- online, 1- offline        
                    rts.IsOnline = (data[0] & 0x08) == 0;
                    break;

                case PhoenixStatusRequests.OffLineStatus:

                    // bit 6: 0- no error, 1- error        
                    rts.HasError = (data[0] & 0x40) != 0;

                    break;

                case PhoenixStatusRequests.ErrorStatus:
                    // bit 3: 0- okay, 1- Not okay    
                    rts.IsCutterOkay = (data[0] & 8) == 0;

                    // bit 5: 0- No fatal (non-recoverable) error, 1- Fatal error        
                    rts.HasFatalError = (data[0] & 8) == 0;

                    // bit 6: 0- No recoverable error, 1- Recoverable error        
                    rts.HasRecoverableError = (data[0] & 0x40) == 1;
                    break;

                case PhoenixStatusRequests.PaperRollStatus:   
                    // bit 5,6: 0- okay, 96- Not okay
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;
                    break;

                default:
                    rts.IsInvalidReport = true;
                    break;
            }

            return ReturnCode.Success;
        }  
    }
}
