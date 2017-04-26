namespace ThermalTalk.Imaging
{
    using System;

    public interface IPrintLogo
    {
        void ApplyColorInversion();
        void ApplyDithering(Algorithms algorithm, byte threshhold);
        string AsBase64String();
        LogoSize Dimensions { get; }
        void Dispose();
        int ForceWidth { get; }
        int IdealHeight { get; }
        int IdealWidth { get; }
        System.Windows.Media.Imaging.BitmapImage ImageData { get; }
        bool IsInverted { get; }
        int MaxHeight { get; }
        int MaxWidth { get; }
        uint TransmitCRC { get; set; }
        string TransmitPath { get; set; }
    }
}
