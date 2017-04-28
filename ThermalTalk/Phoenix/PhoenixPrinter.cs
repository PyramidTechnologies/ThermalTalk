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

    public class PhoenixPrinter : BasePrinter
    {
               
        const int DefaultReadTimeout = 1500; /// ms
        const int DefaultBaudRate = 9600;

        /// <summary>
        /// Constructs a new instance of ReliancePrinter. This printer
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
            FormFeedCommand = new byte[] { 0x0D, 0x0A };
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
        /// Phoenix does not currently supports ESC/POS images at this time.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="doc"></param>
        /// <param name="index"></param>
        public override void SetImage(PrinterImage image, IDocument doc, int index)
        {
            while (index > doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new StandardSection()
            {
                Content = "Image section is not supported on Phoenix"                
            };
        }

        /// <summary>
        /// Phoenix support normal and double scalars. All other scalar values will
        /// be ignored.
        /// </summary>
        /// <param name="w">New scalar (1x, 2x, nop)</param>
        /// <param name="h">New scalar (1x, 2x, nop)</param>
        public override void SetScalars(FontWidthScalar w, FontHeighScalar h)
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
                base.SetScalars(newWidth, newHeight);
            }
        }

        /// <summary>
        /// This command is processed in real time. The reply to this command is sent
        /// whenever it is received and does not wait for previous ESC/POS commands to be executed first.
        /// If there is no response or an invalid response, all fields of RealTimeStatus will be null.
        /// </summary>
        /// <param name="r">StatusRequest type</param>
        /// <returns>Instance of PhoenixStatus,m null on failure, Unset fields will be null</returns>
        public PhoenixStatus GetStatus(PhoenixStatusRequests r)
        {
            // Result stored here
            PhoenixStatus rts = null;

            // Send the real time status command, r is the argument
            var command = new byte[] { 0x10, 0x04, (byte)r };
            int respLen = 1;

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
            if (data.Length != respLen)
            {
                return rts;
            }

            rts = new PhoenixStatus();

            switch (r)
            {
                case PhoenixStatusRequests.Status:
                    // bit 3: 0- online, 1- offline        
                    rts.IsOnline = (data[0] & 0x08) == 0;
                    break;

                case PhoenixStatusRequests.OffLineStatus:
                    // bit 2: 0- no error, 1- error        
                    rts.IsCoverClosed = (data[0] & 0x04) == 0;

                    // bit 3: 0- no error, 1- error        
                    rts.IsNormalFeed = (data[0] & 0x08) == 0;

                    // bit 5: 0- no error, 1- error        
                    rts.IsPaperPresent = (data[0] & 0x20) == 0;

                    // bit 6: 0- no error, 1- error        
                    rts.HasError = (data[0] & 0x40) == 0;

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
                    /// bit 2,3: 0- okay, 12- Not okay        
                    rts.IsPaperLevelOkay = (data[0] & 0x0C) == 0;

                    /// bit 5,6: 0- okay, 96- Not okay
                    rts.IsPaperPresent = (data[0] & 0x60) == 0;
                    break;               
            }

            return rts;
        }
  
    }
}
