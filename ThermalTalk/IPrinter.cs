namespace ThermalTalk
{
    interface IPrinter : System.IDisposable
    {
        /// <summary>
        /// Gets the serial connection to use with this printer
        /// </summary>
        ISerialConnection Connection { get; }

        /// <summary>
        /// Gets the active font effects      
        /// </summary>
        FontEffects Effects { get; }

        /// <summary>
        /// Gets or sets the active justification
        /// </summary>
        FontJustification Justification { get; }

        /// <summary>
        /// Gets or Sets the font's height scalar        
        /// </summary>
        FontHeighScalar Height { get; }

        /// <summary>
        /// Gets or Sets the font's width scalar
        /// </summary>
        FontWidthScalar Width { get; }

        /// <summary>
        /// Applies the specified scalars
        /// </summary>
        /// <param name="w">Width scalar</param>
        /// <param name="h">Height scalar</param>
        void SetScalars(FontWidthScalar w, FontHeighScalar h);

        /// <summary>
        /// Applies the specified justification
        /// </summary>
        /// <param name="justification">Justification to use</param>
        void SetJustification(FontJustification justification);

        /// <summary>
        /// Activates effect for next print. This effect
        /// may be bitwise OR'd to apply multiple effects at
        /// one time. If there are any conflicting effects, the
        /// printer has final say on the defined behavior. 
        /// </summary>
        /// <param name="effect">Font effect to apply</param>
        void AddEffect(FontEffects effect);

        /// <summary>
        /// Remove effect from the active effect list. If effect
        /// is not currently in the list of active effects, nothing
        /// will happen.
        /// </summary>
        /// <param name="effect">Effect to remove</param>
        void RemoveEffect(FontEffects effect);

        /// <summary>
        /// Remove all effects immediately. Only applies
        /// to data that has not yet been transmitted.
        /// </summary>
        void ClearAllEffects();

        /// <summary>
        /// Sets all ESC/POS options to default
        /// </summary>
        void Reinitialize();

        /// <summary>
        /// Print str as ASCII text.
        /// </summary>
        /// <param name="str"></param>
        void PrintASCIIString(string str);

        /// <summary>
        /// Emit one newline character and return print
        /// position to start of line.
        /// </summary>
        void PrintNewline();

        /// <summary>
        /// Mark ticket as complete and present
        /// </summary>
        void FormFeed();

        /// <summary>
        /// Send raw buffer to target printer.
        /// </summary>
        /// <param name="raw"></param>
        void SendRaw(byte[] raw);
    }
}
