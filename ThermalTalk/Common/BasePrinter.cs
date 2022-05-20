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
    using System.IO;
    using System.Text;
    using ThermalTalk.Imaging;

    /// <summary>
    /// A buffered printing interface that builds a document in memory. This document is referred to
    /// as the document buffer. To print your document buffer, call <see cref="FormFeed"/>.
    /// </summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public abstract class BasePrinter : IPrinter
    {
        protected MemoryStream _stream;
        protected BinaryWriter _docBuffer;

        /// <summary>
        /// Initialize common printer properties
        /// </summary>
        protected BasePrinter()
        {
            Justification = FontJustification.JustifyLeft;
            SetScalarCommand = new byte[0];
            InitPrinterCommand = new byte[0];
            FormFeedCommand = new byte[0];
            NewLineCommand = new byte[0];

            _stream = new MemoryStream();
            _docBuffer = new BinaryWriter(_stream);
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
        /// Map justification commands and the specific byte command to apply them
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

        /// <inheritdoc />
        /// <summary>
        /// Gets the active font
        /// </summary>
        public ThermalFonts Font { get; private set; }

        /// <inheritdoc />
        public ILogger Logger { get; set; }

        /// <summary>
        /// Encodes the specified string as a center justified 2D barcode. 
        /// This 2D barcode is compliant with the QR Code® specicification and can be read by all 2D barcode readers.
        /// </summary>
        /// <param name="encodeThis">String to encode</param>
        public abstract ReturnCode Print2DBarcode(string encodeThis);    

        /// <inheritdoc />
        /// <summary>
        /// Sets the active font to this
        /// </summary>
        /// <param name="font">Font to use</param>
        public abstract ReturnCode SetFont(ThermalFonts font);

        /// <inheritdoc />
        /// <summary>
        /// Returns the sepcified status report for this printer
        /// </summary>
        /// <param name="type">Status query type</param>
        /// <returns>Status report</returns>
        public abstract StatusReport GetStatus(StatusTypes type);

        /// <inheritdoc />
        /// <summary>
        /// Send the ESC/POS reinitialize command which restores all 
        /// default options, configurable, etc.
        /// </summary>
        public virtual ReturnCode Reinitialize()
        {
            Logger?.Trace("Restoring all default options and configurable . . .");
            
            Justification = FontJustification.JustifyLeft;
            Width = FontWidthScalar.w1;
            Height = FontHeighScalar.h1;
            Effects = FontEffects.None;

            return AppendToDocBuffer(InitPrinterCommand);
        }

        /// <inheritdoc />
        /// <summary>
        /// Applies the specified scalars
        /// </summary>
        /// <param name="w">Width scalar</param>
        /// <param name="h">Height scalar</param>
        public virtual ReturnCode SetScalars(FontWidthScalar w, FontHeighScalar h)
        {
            Logger?.Trace("Setting font scalars . . .");
            
            // If both scalars are set to "keep current" then do nothing
            if(w == FontWidthScalar.NOP && h == FontHeighScalar.NOP)
            {
                Logger?.Info("Both scalars are set to keep current . . .");
                return ReturnCode.Success;
            }

            // Do not alter the scalars if param is set to x0 which means
            // "keep the current scalar"
            Width = w == FontWidthScalar.NOP ? Width : w;
            Height = h == FontHeighScalar.NOP ? Height : h;

            byte wb = (byte)w;
            byte hb = (byte)h;

            byte[] cmd = (byte[])SetScalarCommand.Clone();

            cmd[2] = (byte)(wb | hb);
            return AppendToDocBuffer(cmd);
        }

        /// <inheritdoc />
        /// <summary>
        /// Applies the specified justification
        /// </summary>
        /// <param name="justification">Justification to use</param>
        public virtual ReturnCode SetJustification(FontJustification justification)
        {
            Logger?.Trace("Setting justification . . .");
            
            // If "keep current" justification is set, do nothing
            if(justification == FontJustification.NOP)
            {
                Logger?.Trace("Justification set to do nothing . . .");
                return ReturnCode.Success;
            }

            Justification = justification;

            if (JustificationCommands.ContainsKey(justification))
            {
                byte[] cmd = JustificationCommands[justification];
                if (cmd != null)
                {
                    return AppendToDocBuffer(cmd);
                }
            }

            return ReturnCode.ExecutionFailure;
        }

        /// <inheritdoc />
        public virtual ReturnCode AddEffect(FontEffects effect)
        {
            Logger?.Trace("Adding font effects . . .");

            var result = ReturnCode.Success;
            
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                if (!EnableCommands.ContainsKey(flag))
                {
                    continue;
                }
                var cmd = EnableCommands[flag];
                if (cmd.Length > 0)
                {
                    var ret = AppendToDocBuffer(cmd);

                    if (ret != ReturnCode.Success)
                    {
                        result = ReturnCode.ExecutionFailure;
                    }
                }
            }

            Effects |= effect;

            return result;
        }

        /// <inheritdoc />
        public virtual ReturnCode RemoveEffect(FontEffects effect)
        {
            Logger?.Trace("Removing font effects . . .");
            
            var result = ReturnCode.Success;
            
            foreach (var flag in effect.GetFlags())
            {
                // Lookup enable command and send if non-empty
                if (DisableCommands.ContainsKey(flag))
                {
                    var cmd = DisableCommands[flag];
                    if (cmd.Length > 0)
                    {
                        var ret = AppendToDocBuffer(cmd);
                        
                        if (ret != ReturnCode.Success)
                        {
                            Logger?.Error("Failed to remove all effects . . .");
                            result = ReturnCode.ExecutionFailure;
                        }
                    }
                }
            }
            Effects &= ~effect;

            return result;
        }

        /// <inheritdoc />
        public virtual ReturnCode ClearAllEffects()
        {
            Logger?.Trace("Clearing all font effects . . .");

            var result = ReturnCode.Success;
            
            foreach (var cmd in DisableCommands.Values)
            {
                if (cmd.Length > 0)
                {
                    var ret = AppendToDocBuffer(cmd);
                    
                    if (ret != ReturnCode.Success)
                    {
                        Logger?.Error("Failed to clear all effects . . .");
                        result = ReturnCode.ExecutionFailure;
                    }
                }
            }
            Effects = FontEffects.None;

            return result;
        }

        /// <inheritdoc />
        public virtual ReturnCode PrintASCIIString(string str)
        {
            Logger?.Trace("Printing the following ASCII string: " + str);
            
            return AppendToDocBuffer(Encoding.ASCII.GetBytes(str));
        }

        /// <inheritdoc />
        public virtual ReturnCode PrintDocument(IDocument doc)
        {
            Logger?.Trace("Printing document . . .");

            // Keep track of current settings so we can restore
            var oldJustification = Justification;
            var oldWidth = Width;
            var oldHeight = Height;
            var oldFont = Font;
            
            foreach (var sec in doc.Sections)
            {

                // First apply all effects. The firmware decides if any there
                // are any conflicts and there is nothing we can do about that.
                // Apply the rest of the settings before we send string
                var subResults = new List<ReturnCode>
                {
                    AddEffect(sec.Effects),
                    SetJustification(sec.Justification),
                    SetScalars(sec.WidthScalar, sec.HeightScalar),
                    SetFont(sec.Font),

                    // Send the actual content
                    AppendToDocBuffer(sec.GetContentBuffer(doc.CodePage)),
                };


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

            return ReturnCode.Success;
        }

        /// <inheritdoc />
        public abstract ReturnCode SetImage(PrinterImage image, IDocument doc, int index);

        /// <inheritdoc />
        public ReturnCode SetBarcode(IBarcode barcode, IDocument doc, int index)
        {
            while (index >= doc.Sections.Count)
            {
                doc.Sections.Add(new Placeholder());
            }

            doc.Sections[index] = new BarcodeSection(barcode);

            return ReturnCode.Success;
        }

        /// <inheritdoc />
        public virtual ReturnCode PrintNewline()
        {
            Logger?.Trace("Printing new line . . . ");
            
            return AppendToDocBuffer(NewLineCommand);
        }

        /// <summary>
        /// Writes document buffer to printer and clears the print buffer.
        /// </summary>
        /// <returns><see cref="ReturnCode.UnsupportedCommand"/> if the document buffer is empty.
        /// If the document fails to transmit, <see cref="ReturnCode.ExecutionFailure"/> is returned.
        /// Otherwise, <see cref="ReturnCode.Success"/></returns>
        public virtual ReturnCode FormFeed()
        {
            Logger?.Trace("Marking ticket as complete and presenting");
            
            AppendToDocBuffer(FormFeedCommand);
            
            var payload = _stream.ToArray();
            
            // Do not send empty packets
            if (payload.Length == 0)
            {
                Logger?.Warn("Warning: payload is empty");
                return ReturnCode.UnsupportedCommand;
            }
            
            try
            {
                Logger?.Trace("Attempting to open connection");
                Connection.Open();

                Connection.Write(payload);
                
                return ReturnCode.Success;
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
                // BinaryWriter closes the underlying stream on dispose 
                _docBuffer.Dispose();
                _stream = new MemoryStream();
                _docBuffer = new BinaryWriter(_stream);
                
                Logger?.Trace("Closing connection");
                Connection.Close();
            }
        }

        /// <inheritdoc />
        public virtual ReturnCode SendRaw(byte[] raw)
        {
            Logger?.Trace("Sending raw data . . .");

            return AppendToDocBuffer(raw);
        }


        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Close serial connection
        /// </summary>
        /// <param name="disposing">True to close connection</param>
        protected virtual void Dispose(bool disposing)
        {
            if (Connection != null)
            {
                Connection.Dispose();
            }
            
            _docBuffer.Dispose();
        }

        #region Protected
        /// <summary>
        /// Append print command data to the end of the current document.
        /// Document is not actually printed until <see cref="FormFeed"/> is called.
        /// </summary>
        /// <param name="payload"></param>
        /// <returns>ReturnCode.Success if successful, ReturnCode.UnsupportedCommand if payload.Length == 0,
        /// and ReturnCode.ExecutionFailure otherwise.</returns>
        protected ReturnCode AppendToDocBuffer(byte[] payload)
        {            
            if (payload is null)
            {
                Logger.Error($"Attempted to write null payload to document in {nameof(AppendToDocBuffer)}");
                return ReturnCode.InvalidArgument;
            }
            
            _docBuffer.Write(payload);
            return ReturnCode.Success;
        }
        #endregion
    }
}
