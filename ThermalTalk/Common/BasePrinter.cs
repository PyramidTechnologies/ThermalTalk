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
namespace ThermalTalk
{
    using System.Collections.Generic;
    using System.Text;

    public abstract class BasePrinter : IPrinter
    {

        protected BasePrinter()
        {
            SetScalarCommand = new byte[0];
            InitPrinterCommand = new byte[0];
            FormFeedCommand = new byte[0];
            NewLineCommand = new byte[0];
        }

        /// <summary>
        /// Destructor - Close and dispose serial port if needed
        /// </summary>
        ~BasePrinter()
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        /// <summary>
        /// Gets the serial connection for this device
        /// </summary>
        protected virtual ISerialConnection Connection { get; set; }

        /// <summary>
        /// Command to apply scalar. Add extra 0 byte to hold the configuration value
        /// Leave empty if not supported.       
        /// </summary>
        protected virtual byte[] SetScalarCommand { get; set; }

        /// <summary>
        /// Command sent to initialize printer. 
        /// Leave empty if not supported.
        /// </summary>
        protected virtual byte[] InitPrinterCommand { get; set; }

        /// <summary>
        /// Command sent to execute a newline and print job
        /// Leave empty if not supported.
        /// </summary>
        protected virtual byte[] FormFeedCommand { get; set; }

        /// <summary>
        /// Command sent to execute a newline
        /// Leave empty if not supported.
        /// </summary>
        protected virtual byte[] NewLineCommand { get; set; }


        /// <summary>
        /// Map of font effects and the specific byte command to apply them
        /// </summary>
        protected abstract Dictionary<FontEffects, byte[]> EnableCommands { get; set; }

        /// <summary>
        /// Map of font effects and the specific byte command to de-apply them
        /// </summary>
        protected abstract Dictionary<FontEffects, byte[]> DisableCommands { get; set; }

        /// <summary>
        /// Map justifcation commands and the specific byte command to apply them
        /// </summary>
        protected abstract Dictionary<FontJustification, byte[]> JustificationCommands { get; set; }

        /// <summary>
        /// Gets or sets the read timeout in milliseconds
        /// </summary>
        public int PrintSerialReadTimeout { get; set; }

        /// <summary>
        /// Gets or sets the serial port baud rate
        /// </summary>
        public int PrintSerialBaudRate { get; set; }

        /// <summary>
        /// Gets or Sets the font's height scalar        
        /// </summary>
        public FontHeighScalar Height { get; private set; }

        /// <summary>
        /// Gets or Sets the font's width scalar
        /// </summary>
        public FontWidthScalar Width { get; private set; }

        /// <summary>
        /// Gets the active font effects      
        /// </summary>
        public FontEffects Effects { get; private set; }

        /// <summary>
        /// Gets or sets the active justification
        /// </summary>
        public FontJustification Justification { get; private set; }

        /// <summary>
        /// Send the ESC/POS reinitialize command which restores all 
        /// default options, configurable, etc.
        /// </summary>
        public void Reinitialize()
        {
            Justification = FontJustification.JustifyLeft;
            Width = FontWidthScalar.w1;
            Height = FontHeighScalar.h1;
            Effects = FontEffects.None;

            internalSend(InitPrinterCommand);
        }

        /// <summary>
        /// Applies the specified scalars
        /// </summary>
        /// <param name="w">Width scalar</param>
        /// <param name="h">Height scalar</param>
        public void SetScalars(FontWidthScalar w, FontHeighScalar h)
        {
            Width = w;
            Height = h;

            byte wb = (byte)w;
            byte hb = (byte)h;

            byte[] cmd = (byte[])SetScalarCommand.Clone();

            cmd[2] = (byte)(wb | hb);
            internalSend(cmd);
        }

        /// <summary>
        /// Applies the specified justification
        /// </summary>
        /// <param name="justification">Justification to use</param>
        public void SetJustification(FontJustification justification)
        {
            Justification = justification;

            if (JustificationCommands.ContainsKey(justification))
            {
                byte[] cmd = JustificationCommands[justification];
                if (cmd != null)
                {
                    internalSend(cmd);
                }
            }
        }

        public void AddEffect(FontEffects effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                if (EnableCommands.ContainsKey(flag))
                {
                    var cmd = EnableCommands[flag];
                    if (cmd.Length > 0)
                    {
                        internalSend(cmd);
                    }
                }
            }

            Effects |= effect;
        }

        public void RemoveEffect(FontEffects effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                if (DisableCommands.ContainsKey(flag))
                {
                    var cmd = DisableCommands[flag];
                    if (cmd.Length > 0)
                    {
                        internalSend(cmd);
                    }
                }
            }
            Effects &= ~effect;
        }



        public void ClearAllEffects()
        {
            foreach (var cmd in DisableCommands.Values)
            {
                if (cmd.Length > 0)
                {
                    internalSend(cmd);
                }
            }
            Effects = FontEffects.None;
        }

        public void PrintASCIIString(string str)
        {
            internalSend(ASCIIEncoding.ASCII.GetBytes(str));
        }

        public void PrintDocument(IDocument doc)
        {
            // Keep track of current settings so we can restore
            var oldJustification = Justification;
            var oldWidth = Width;
            var oldHeight = Height;

            foreach (var sec in doc.Sections)
            {

                // First apply all effects. The firwmare decides if any there
                // are any conflicts and there is nothing we can do about that.
                // Apply the rest of the settings before we send string
                AddEffect(sec.Effects);
                SetJustification(sec.Justification);
                SetScalars(sec.WidthScalar, sec.HeightScalar);

                // Send the actual content
                internalSend(sec.GetContentBuffer());

                if (sec.AutoNewline)
                {
                    PrintNewline();
                }

                // Undo all the settings we just set
                RemoveEffect(sec.Effects);
            }

            SetJustification(oldJustification);
            SetScalars(oldWidth, oldHeight);
        }

        public void PrintNewline()
        {
            internalSend(NewLineCommand);
        }

        public void FormFeed()
        {
            internalSend(FormFeedCommand);
        }

        public void SendRaw(byte[] raw)
        {
            internalSend(raw);
        }


        public void Dispose()
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        #region Private
        protected void internalSend(byte[] payload)
        {
            // Do not send empty packets
            if(payload.Length == 0)
            {
                return;
            }

            try
            {
                Connection.Open();

                Connection.Write(payload);
            }
            catch { }
            finally
            {
                Connection.Close();
            }
        }
        #endregion
    }
}
