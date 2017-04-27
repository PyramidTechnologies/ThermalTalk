namespace ThermalTalk
{
    /// <summary>
    /// An implementation with zero side-effects
    /// </summary>
    public class Placeholder : ISection
    {
        public string Content { get { return string.Empty; } set { } }

        public FontEffects Effects { get { return FontEffects.None; } set { } }
   
        public FontJustification Justification { get { return FontJustification.JustifyLeft; } set { } }
   
        public FontWidthScalar WidthScalar { get { return FontWidthScalar.w1; } set { } }

        public FontHeighScalar HeightScalar { get { return FontHeighScalar.h1; } set { } }

        public bool AutoNewline { get { return false; } set { } }

        public byte[] GetContentBuffer()
        {
            return new byte[0];
        }
    }
}
