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
    using System;
    using System.Collections.Generic;
    using System.Text;
    using ThermalTalk.Imaging;

    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BasePrinter : IPrinter
    {
        protected BasePrinter()
        {
            Justification = FontJustification.JustifyLeft;
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
            Dispose(false);
        }

        /// <summary>
        /// Gets the serial connection for this device
        /// </summary>
        protected ISerialConnection Connection { get; set; }

        /// <summary>
        /// Command to apply scalar. Add extra 0 byte to hold the configuration value
        /// Leave empty if not supported.       
        /// </summary>
        protected byte[] SetScalarCommand { get; set; }

        /// <summary>
        /// Command sent to initialize printer. 
        /// Leave empty if not supported.
        /// </summary>
        protected byte[] InitPrinterCommand { get; set; }

        /// <summary>
        /// Command sent to execute a newline and print job
        /// Leave empty if not supported.
        /// </summary>
        protected byte[] FormFeedCommand { get; set; }

        /// <summary>
        /// Command sent to execute a newline
        /// Leave empty if not supported.
        /// </summary>
        protected byte[] NewLineCommand { get; set; }


        /// <summary>
        /// Map of font effects and the specific byte command to apply them
        /// </summary>
        protected Dictionary<FontEffects, byte[]> EnableCommands { get; set; }

        /// <summary>
        /// Map of font effects and the specific byte command to de-apply them
        /// </summary>
        protected Dictionary<FontEffects, byte[]> DisableCommands { get; set; }

        /// <summary>
        /// Map justifcation commands and the specific byte command to apply them
        /// </summary>
        protected Dictionary<FontJustification, byte[]> JustificationCommands { get; set; }

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
        /// Gets the active font
        /// </summary>
        public Fonts Font { get; private set; }

        /// <summary>
        /// Sets the active font to this
        /// </summary>
        /// <param name="font">Font to use</param>
        public abstract void SetFont(Fonts font);

        /// <summary>
        /// Returns the sepcified status report for this printer
        /// </summary>
        /// <param name="type">Status query type</param>
        /// <returns>Status report</returns>
        public abstract StatusReport GetStatus(StatusTypes type);

        /// <summary>
        /// Send the ESC/POS reinitialize command which restores all 
        /// default options, configurable, etc.
        /// </summary>
        public virtual void Reinitialize()
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
        public virtual void SetScalars(FontWidthScalar w, FontHeighScalar h)
        {
            // If both scalars are set to "keep current" then do nothing
            if(w == FontWidthScalar.NOP && h == FontHeighScalar.NOP)
            {
                return;
            }

            // Do not alter the scalars if param is set to x0 which means
            // "keep the current scalar"
            Width = w == FontWidthScalar.NOP ? Width : w;
            Height = h == FontHeighScalar.NOP ? Height : h;

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
        public virtual void SetJustification(FontJustification justification)
        {
            // If "keep current" justification is set, do nothing
            if(justification == FontJustification.NOP)
            {
                return;
            }

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

        public virtual void AddEffect(FontEffects effect)
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

        public virtual void RemoveEffect(FontEffects effect)
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

        public virtual void ClearAllEffects()
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

        public virtual void PrintASCIIString(string str)
        {
            internalSend(ASCIIEncoding.ASCII.GetBytes(str));
        }

        public virtual void PrintDocument(IDocument doc)
        {
            // Keep track of current settings so we can restore
            var oldJustification = Justification;
            var oldWidth = Width;
            var oldHeight = Height;
            var oldFont = Font;

            foreach (var sec in doc.Sections)
            {

                // First apply all effects. The firwmare decides if any there
                // are any conflicts and there is nothing we can do about that.
                // Apply the rest of the settings before we send string
                AddEffect(sec.Effects);
                SetJustification(sec.Justification);
                SetScalars(sec.WidthScalar, sec.HeightScalar);
                SetFont(sec.Font);

                // Send the actual content
                internalSend(sec.GetContentBuffer());

                if (sec.AutoNewline)
                {
                    PrintNewline();
                }

                // Remove effects for this section
                RemoveEffect(sec.Effects);
            }

            // Undo all the settings we just set
            SetJustification(oldJustification);
            SetScalars(oldWidth, oldHeight);
            SetFont(oldFont);
        }

        public abstract void SetImage(PrinterImage image, IDocument doc, int index);

        public virtual void PrintNewline()
        {
            internalSend(NewLineCommand);
        }

        public virtual void FormFeed()
        {
            internalSend(FormFeedCommand);
        }

        public virtual void SendRaw(byte[] raw)
        {
            internalSend(raw);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool diposing)
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
        }

        #region Protected
        protected void internalSend(byte[] payload)
        {
            // Do not send empty packets
            if (payload.Length == 0)
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
