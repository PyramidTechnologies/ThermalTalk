namespace ThermalTalk
{
    using System;

    /// <summary>
    /// Standard document implementation
    /// </summary>
    public class StandardDocument : IDocument
    {
        public string Content { get; set; }

        public FontEffects Effects { get; set; }

        public FontJustification Justification { get; set; }

        public FontWidthScalar WidthScalar { get; set; }

        public FontHeighScalar HeightScalar { get; set; }

        public bool AutoNewline { get; set; }
    }
}
