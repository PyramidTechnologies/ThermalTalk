﻿#region Copyright & License
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

using System;

namespace ThermalTalk
{
    using System.Collections.Generic;
    using System.Text;
    using ThermalTalk.Imaging;

    /// <inheritdoc />
    /// <summary>
    /// Reliance Printer is the primary handle for accessing the printer API
    /// </summary>
    public class ReliancePrinter : BasePrinter
    {
        const int DefaultReadTimeout = 1000; /// ms
        const int DefaultBaudRate = 19200;

        private readonly byte[] CPI11 = { 0x1B, 0xC1, 0x00 };
        private readonly byte[] CPI15 = { 0x1B, 0xC1, 0x01 };
        private readonly byte[] CPI20 = { 0x1B, 0xC1, 0x02 };

        /// <summary>
        /// Constructs a new instance of ReliancePrinter. This printer
        /// acts as a handle to all features and functions. If the serial port parameter
        /// is provided, the serial connection will be opened immediately.
        /// </summary>
        /// <param name="serialPortName">OS name of serial port</param>        
        public ReliancePrinter(string serialPortName)
        {
            Logger?.Trace("Creating new instance of Reliance Printer on port: " + serialPortName);

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
            FormFeedCommand = new byte[] { 0x0C };
            NewLineCommand = new byte[] { 0x0A };
            InitPrinterCommand = new byte[] { 0x1B, 0x40 };

            // User wants a serial port
            if (!string.IsNullOrEmpty(serialPortName))
            {            
                PrintSerialReadTimeout = DefaultReadTimeout;
                PrintSerialBaudRate = DefaultBaudRate;

                Connection = new RelianceSerialPort(serialPortName, PrintSerialBaudRate)
                {
                    ReadTimeoutMS = DefaultReadTimeout
                };

            }
        }

        /// <summary>
        /// Sets the active font to this
        /// </summary>
        /// <param name="font">Font to use</param>
        /// /// <returns>ReturnCode.Success if successful, ReturnCode.ExecutionFailure otherwise.</returns>
        public override ReturnCode SetFont(ThermalFonts font)
        {
            Logger?.Trace("Setting thermal fonts . . .");
            
            if (font == ThermalFonts.NOP)
            {
                Logger?.Trace("No change selected");
                
                return ReturnCode.Success;
            }

            var result = ReturnCode.ExecutionFailure;

            // A == 11 CPI
            // B == 15 CPI
            // C == 20 CPI
            // Just change the CPI mode and then leave at Font A to avoid 
            // having multiple configurables to set/clear
            switch (font)
            {
                case ThermalFonts.A:
                    Logger?.Trace("Attempting to set font to font CPI11.");
                    result = AppendToDocBuffer(new BufferAction
                    {
                        Buffer = CPI11,
                    });
                    break;
                case ThermalFonts.B:
                    Logger?.Trace("Attempting to set font to font CPI15.");
                    result = AppendToDocBuffer(new BufferAction
                    {
                        Buffer = CPI15,
                    });
                    break;
                case ThermalFonts.C:
                    Logger?.Trace("Attempting to set font to font CPI20.");
                    result = AppendToDocBuffer(new BufferAction
                    {
                        Buffer = CPI20,
                    });
                    break;

            }

            return result;
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
        /// <returns>ReturnCode.Success if successful, ReturnCode.ExecutionFailure otherwise.</returns>
        public override ReturnCode Print2DBarcode(string encodeThis)
        {
            // Use all default values for barcode
            var barcode = new TwoDBarcode(TwoDBarcode.Flavor.Reliance)
            {
                EncodeThis = encodeThis
            };
            var fullCmd = barcode.Build();
            return AppendToDocBuffer(new BufferAction
            {
                Buffer = fullCmd,
                AfterSendDelay = TimeSpan.FromMilliseconds(500)
            });
        }

        /// <summary>
        /// Build and send provided barcode. If this is too limiting,
        /// feel free to follow the docs and build your own payload that
        /// can be sent with the #SendRaw method.
        /// </summary>
        /// <param name="barcode">Barcode object</param>
        /// <returns>ReturnCode.Success if successful, ReturnCode.ExecutionFailure if there is an issue
        /// writing the barcode. Returns ReturnCode.InvalidArgument if the barcode.Build() is not
        /// a positive length.</returns>
        public ReturnCode PrintBarcode(IBarcode barcode)
        {
            var payload = barcode.Build();
            if (payload.Length > 0)
            {
                return AppendToDocBuffer(new BufferAction
                {
                    Buffer = payload,
                    AfterSendDelay = TimeSpan.FromMilliseconds(500)
                });
            }

            Logger?.Error("barcode.Build() has length 0.");
            return ReturnCode.InvalidArgument;
        }

        /// <inheritdoc />
        public override ReturnCode SetImage(PrinterImage image, IDocument doc, int index)
        {
            while(index >= doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new RelianceImageSection() {
                Image = image,
            };

            return ReturnCode.Success;
        }

        /// <inheritdoc />
        /// <summary>
        /// This command is processed in real time. The reply to this command is sent
        /// whenever it is received and does not wait for previous ESC/POS commands to be executed first.
        /// If there is no response or an invalid response, If there is a read timeout or comm failure, the
        /// result will have the IsValidReport set to false.
        /// </summary>
        /// <param name="type">StatusRequest type</param>
        /// <returns>Instance of RelianceStatus</returns>
        public override StatusReport GetStatus(StatusTypes type)
        {
            // Translate generic status to phoenix status
            RelianceStatusRequests r;
            switch (type)
            {
                case StatusTypes.PrinterStatus:
                    r = RelianceStatusRequests.Status;
                    break;

                case StatusTypes.OfflineStatus:
                    r = RelianceStatusRequests.OffLineStatus;
                    break;

                case StatusTypes.ErrorStatus:
                    r = RelianceStatusRequests.ErrorStatus;
                    break;

                case StatusTypes.PaperStatus:
                    r = RelianceStatusRequests.PaperRollStatus;
                    break;

                case StatusTypes.MovementStatus:
                    r = RelianceStatusRequests.PrintStatus;
                    break;

                case StatusTypes.FullStatus:
                    r = RelianceStatusRequests.FullStatus;
                    break;

                default:
                    // Unknown status type
                    return StatusReport.Invalid();
            }

            // Send the real time status command, r is the argument
            var command = new byte[] { 0x10, 0x04, (byte)r };
            var respLen = (r == RelianceStatusRequests.FullStatus) ? 6 : 1;

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
            { /* Do nothing */ }
            finally
            {
                Connection.Close();
            }
            
            // Invalid response
            if(data.Length != respLen)
            {
                return StatusReport.Invalid();
            }

            var rts = new StatusReport();

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
                    rts.HasError = (data[0] & 0x40) == 0x40;
                 
                    break;

                case RelianceStatusRequests.ErrorStatus:
                    // bit 3: 0- okay, 1- Not okay    
                    rts.IsCutterOkay = (data[0] & 8) == 0;    

                    // bit 5: 0- No fatal (non-recoverable) error, 1- Fatal error        
                    rts.HasFatalError = (data[0] & 0x20) == 0x20;    
    
                    // bit 6: 0- No recoverable error, 1- Recoverable error        
                    rts.HasRecoverableError = (data[0] & 0x40) == 0x40; 
                    break;

                case RelianceStatusRequests.PaperRollStatus:
                    // bit 2,3: 0- okay, 12- Not okay        
                    rts.IsPaperLevelOkay = (data[0] & 0x0C) == 0;

                    // bit 5,6: 0- okay, 96- Not okay
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;                    
                    break;

                case RelianceStatusRequests.PrintStatus:
                    // bit 2: 0- motor off, 1: motor on        
                    rts.IsPaperMotorOff = (data[0] & 4) == 0;
                    // bit 5: 0- paper present, 1: motor stopped because out of paper
                    
                    rts.IsPaperPresent = (data[0] & 4) == 0;
                    break;

                case RelianceStatusRequests.FullStatus:

                    rts.IsPaperPresent = (data[2] & 0x01) == 0;
                    rts.IsPaperLevelOkay = (data[2] & 0x04) == 0;
                    rts.IsTicketPresentAtOutput = (data[2] & 0x20) == 0x20;

                    // Custom specs duplicates these so if EITHER
                    // are set to open, report open. 0: closed, 1: open
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

                default:
                    rts.IsInvalidReport = true;
                    break;
            }

            return rts;
        }
    }
}
