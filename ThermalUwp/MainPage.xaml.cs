using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using ThermalTalk;
using ThermalTalk.Imaging;
using ThermalUwp.Serial;

namespace ThermalUwp
{
    public sealed partial class MainPage : Page
    {
        private static readonly List<string> ImageExtensions = new List<string>
            { ".jpg", ".jpeg", ".jpe", ".png", ".bmp", ".gif" };

        private static readonly FileOpenPicker FilePicker;

        private readonly Dictionary<string, SerialDevice> _serialDevices = new Dictionary<string, SerialDevice>();
        private readonly StandardSection _header;
        private readonly StandardSection _timestamp;
        private readonly StandardSection _printStatus;
        private readonly StandardDocument _document;
        private BasePrinter _printer;
        private string _portName = "";
        private string _imagePath = "";
        private SKBitmap _bitmap;

        static MainPage()
        {
            FilePicker = new FileOpenPicker
            {
                ViewMode = PickerViewMode.Thumbnail,
                SuggestedStartLocation = PickerLocationId.PicturesLibrary
            };

            foreach (var extension in ImageExtensions)
                FilePicker.FileTypeFilter.Add(extension);
        }

        public MainPage()
        {
            InitializeComponent();
            LoadSerialDevices();

            // Setup the header with double width, double height, and justified center.
            _header = new StandardSection
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h2,
                WidthScalar = FontWidthScalar.w2,
                Font = ThermalFonts.A,
                AutoNewline = true,
            };

            // Setup timestamp at normal scalar with bold, underline, and centered.
            _timestamp = new StandardSection
            {
                Justification = FontJustification.JustifyCenter,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Effects = FontEffects.Italic | FontEffects.Underline,
                Font = ThermalFonts.B,
                AutoNewline = true,
            };

            // Print status is shown as a JSON string.
            _printStatus = new StandardSection
            {
                Justification = FontJustification.JustifyLeft,
                HeightScalar = FontHeighScalar.h1,
                WidthScalar = FontWidthScalar.w1,
                Font = ThermalFonts.C,
                AutoNewline = true,
            };

            // Document Template
            // Print #{}
            // Image
            // Barcode
            // Timestamp
            // Print Status
            _document = new StandardDocument
            {
                // Don't forget to set codepage.
                CodePage = CodePages.CPSPACE,
            };

            _document.Sections.Add(_header);
            _document.Sections.Add(new Placeholder()); // Placeholder for an image.
            _document.Sections.Add(new Placeholder()); // Placeholder for a barcode.
            _document.Sections.Add(_timestamp);
            _document.Sections.Add(_printStatus);

            PreviewImage.RegisterPropertyChangedCallback(Image.SourceProperty,
                (sender, dp) =>
                {
                    ImageSelectorLabel.Visibility =
                        PreviewImage.Source == null ? Visibility.Visible : Visibility.Collapsed;
                });
            return;

            async void LoadSerialDevices()
            {
                IsEnabled = false;

                var infos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());
                foreach (var info in infos)
                {
                    var device = await SerialDevice.FromIdAsync(info.Id).AsTask();
                    if (device == null) continue;

                    _serialDevices.Add(device.PortName, device);
                }

                IsEnabled = true;
            }
        }

        private TPrinter GetPrinter<TPrinter>(string portName) where TPrinter : BasePrinter
        {
            if (!_serialDevices.ContainsKey(portName))
                return null;

            var device = _serialDevices[portName];
            if (typeof(TPrinter) == typeof(ReliancePrinter))
                return new ReliancePrinter(new RelianceSerialPort(device)) as TPrinter;
            if (typeof(TPrinter) == typeof(PhoenixPrinter))
                return new PhoenixPrinter(new PhoenixSerialPort(device)) as TPrinter;
            return null;
        }

        private void ImageSelector_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Hand, 1);

            if (_imagePath == "")
                return;

            ImageSelectorCover.Opacity = 0.5f;
            ImageSelectorLabel.Visibility = Visibility.Visible;
        }

        private void ImageSelector_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            Window.Current.CoreWindow.PointerCursor = new CoreCursor(CoreCursorType.Arrow, 1);

            ImageSelectorCover.Opacity = 0f;

            if (_imagePath == "")
                return;

            ImageSelectorLabel.Visibility = Visibility.Collapsed;
        }

        private async void ImageSelector_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var imageFile = await FilePicker.PickSingleFileAsync();
            if (imageFile == null || imageFile.Path == _imagePath)
                return;

            _imagePath = imageFile.Path;

            var fileStream = await imageFile.OpenAsync(FileAccessMode.Read);

            var bitmapImage = new BitmapImage();
            await bitmapImage.SetSourceAsync(fileStream);
            PreviewImage.Source = bitmapImage;

            var dr = new DataReader(fileStream.GetInputStreamAt(0));
            await dr.LoadAsync((uint)fileStream.Size);
            var buffer = new byte[fileStream.Size];
            dr.ReadBytes(buffer);
            _bitmap = SKBitmap.Decode(buffer);

            dr.Dispose();
            fileStream.Dispose();
        }

        private void PrintButton_Click(object sender, RoutedEventArgs e)
        {
            if (_imagePath == "")
            {
                ErrorLabel.Text = "Image is not selected.";
                return;
            }

            // Connect to printer.
            switch (PrinterSelector.SelectedIndex)
            {
                case 0 when !(_printer is ReliancePrinter) || _portName != PortNameInput.Text:
                    _printer?.Dispose();
                    _printer = GetPrinter<ReliancePrinter>(PortNameInput.Text);
                    _portName = PortNameInput.Text;
                    break;
                case 1 when !(_printer is PhoenixPrinter) || _portName != PortNameInput.Text:
                    _printer?.Dispose();
                    _printer = GetPrinter<PhoenixPrinter>(PortNameInput.Text);
                    _portName = PortNameInput.Text;
                    break;
            }

            if (_printer == null || !_printer.GetStatus(StatusTypes.PrinterStatus).IsOnline)
            {
                _printer?.Dispose();
                _printer = null;

                ErrorLabel.Text = "Printer was not found.";
                return;
            }

            ErrorLabel.Text = "";

            var currentTime = DateTime.Now;

            // Print the header document with print number.
            _header.Content = Path.GetFileName(_imagePath);

            // Print the timestamp document.
            _timestamp.Content = currentTime.ToString(CultureInfo.CurrentCulture);

            // Get the latest printer status.
            // Note that reliance and phoenix have slightly different args to this get status command.
            _printStatus.Content = _printer.GetStatus(StatusTypes.FullStatus).ToJSON(true);

            // Re-assign selected image to the middle part of the document.
            // Phoenix does not currently support dynamic images over ESC/POS.
            var image = new PrinterImage(_bitmap);
            image.ApplyDithering(Algorithms.JarvisJudiceNinke, 128);
            _printer.SetImage(image, _document, 1);

            // Update barcode.
            var barcode = _printer is ReliancePrinter
                ? new TwoDBarcode(TwoDBarcode.Flavor.Reliance)
                : new TwoDBarcode(TwoDBarcode.Flavor.Phoenix);
            barcode.EncodeThis = $"Capture occurred at {currentTime}.";
            _printer.SetBarcode(barcode, _document, 2);

            // Send the whole document + image.
            _printer.PrintDocument(_document);
            _printer.FormFeed();

            image.Dispose();
        }
    }
}