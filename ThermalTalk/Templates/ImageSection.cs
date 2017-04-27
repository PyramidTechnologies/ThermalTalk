
namespace ThermalTalk
{
    using ThermalTalk.Imaging;

    public class ImageSection : StandardSection
    {
        /// <summary>
        /// An image has no text content
        /// </summary>
        public override string Content { get { return string.Empty; } set { } }

        /// <summary>
        /// Images do not support font effects
        /// </summary>
        public override FontEffects Effects { get { return FontEffects.None; } set { } }

        /// <summary>
        /// Images do not support width scalar
        /// </summary>
        public override FontWidthScalar WidthScalar { get { return FontWidthScalar.w1; } set { } }

        /// <summary>
        /// Images do no support height scalar
        /// </summary>
        public override FontHeighScalar HeightScalar { get { return FontHeighScalar.h1; } set { } }

        /// <summary>
        /// Image to place inside document
        /// </summary>
        public PrinterImage Image { get; set; }


        public override byte[] GetContentBuffer()
        {
            return Image.GetAsEscBuffer();
        }
    }
}
