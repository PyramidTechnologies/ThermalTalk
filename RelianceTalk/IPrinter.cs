namespace RelianceTalk
{
    interface IPrinter
    {
        /// <summary>
        /// Gets or Sets the serial connection to use with this printer
        /// </summary>
        ISerialConnection Connection { get; set; }

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
        /// Apply a font width and height scalar
        /// </summary>
        /// <param name="w">Width scalar as multiplier</param>
        /// <param name="h">Heigh scalar as multiplier</param>
        void SetFontScalar(FontWidthScalar w, FontHeighScalar h);

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
        /// Send raw buffer to target printer.
        /// </summary>
        /// <param name="raw"></param>
        void SendRaw(byte[] raw);
    }
}
