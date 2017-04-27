namespace ThermalTalk
{
    /// <summary>
    /// An implementation with zero side-effects
    /// </summary>
    public class Placeholder : ISection
    {
        public string Content { get { return string.Empty; } set { } }

        public FontEffects Effects { get { return FontEffects.None; } set { } }
   
        public FontJustification Justification { get { return FontJustification.JustifyNone; } set { } }
   
        public FontWidthScalar WidthScalar { get { return FontWidthScalar.w0; } set { } }

        public FontHeighScalar HeightScalar { get { return FontHeighScalar.h0; } set { } }

        public bool AutoNewline { get { return false; } set { } }

        public byte[] GetContentBuffer()
        {
            return new byte[0];
        }
    }
}
