# 3.0.0.0
**Bug Fixes**
* None

**New**
* Replace `System.Drawing` library with `SkiaSharp`
* Add ability to initialize `ReliancePrinter` and `PhoenixPrinter` with `ISerialConnection` instance
* Add support for .NET 7.0 and 6.0; drop support for .NET Framework 4.5.1
* Combine console samples into 1 app
* Add UWP sample

# 2.1.1.0
** bug fixes **
* None

* New *
* Add image raster command for Phoenix
* Increase write timeout to support printing larger raster images

# 2.1.0.0
** bug fixes **
* None

* New *
* Add BarcodeSection and TwoD barcode type

# 2.0.1.0
** bug fixes **
* #12 Phoenix OffLineStatus corrected

* New *
* Demo program now uses command line args instead of hard coded port name

# 2.0.0.0
** bug fixes **
* None

* New *
* Add support for net451, netstandard2.0, and net5.0

# 1.4
** bug fixes **
* Fixed IsTicketPresentAtOutput flag parse
* Fixed HasError flag parse
* Fixed HasFatalError flag parse
* Fixed HasRecoverableError flag parse

* New *
* Does not assume webcam number in CLI example project

# 1.3
** bug fixes **

* None

*New*

* Phoenix now behaves like Reliance where possible for status reports

# 1.3
** bug fixes **

* Return valid object, not null on comm error

*New*

* Include docs in nuget

# 1.2
** bug fixes **

* None

*New*

* Add support for codepage 771 and 437


# 1.1.1
** bug fixes **

* PrinterImage.Resize properly maintains aspect ratio

*New*

* ThermalTalk lib will always use the latest ThermalTalk.Imaging


# 1.1.0
** bug fixes **

* None

*New*

* Rename Fonts -> ThermalFonts to avoid namespace confusion
* PrinterImage.ImageData is now read-only (frozen)

* Nothing

# 1.0.0
** First Release **