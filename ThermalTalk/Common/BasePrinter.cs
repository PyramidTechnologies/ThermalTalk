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
        protected Dictionary<FontEffects, byte[]> EnableCommands = new Dictionary<FontEffects, byte[]>();
        protected Dictionary<FontEffects, byte[]> DisableCommands = new Dictionary<FontEffects, byte[]>();
        protected Dictionary<FontJustification, byte[]> JustificationCommands = new Dictionary<FontJustification, byte[]>();
        protected byte[] SetScalarCommand = new byte[0];
        protected byte[] InitPrinterCommand = new byte[0];
        protected byte[] FormFeedCommand = new byte[0];
        protected byte[] NewLineCommand = new byte[0];

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
        protected ISerialConnection Connection { get; set; }

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

            byte[] cmd = JustificationCommands[justification];
            if(cmd != null)
            {
                internalSend(cmd);
            }
        }

        public void AddEffect(FontEffects effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                var cmd = EnableCommands[flag];
                if(cmd.Length > 0)
                {
                    internalSend(cmd);
                }
            }

            Effects |= effect;
        }

        public void RemoveEffect(FontEffects effect)
        {
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                var cmd = DisableCommands[flag];
                if (cmd.Length > 0)
                {
                    internalSend(cmd);
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

            // First apply all effects. The firwmare decides if any there
            // are any conflicts and there is nothing we can do about that.
            // Apply the rest of the settings before we send string
            AddEffect(doc.Effects);
            SetJustification(doc.Justification);
            SetScalars(doc.WidthScalar, doc.HeightScalar);

            // Send the actual content
            internalSend(ASCIIEncoding.ASCII.GetBytes(doc.Content));

            if(doc.AutoNewline)
            {
                PrintNewline();
            }

            // Undo all the settings we just set
            RemoveEffect(doc.Effects);
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
