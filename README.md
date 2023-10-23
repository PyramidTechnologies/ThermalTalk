[![NuGet](https://img.shields.io/nuget/v/ThermalTalk.svg)](https://www.nuget.org/packages/ThermalTalk/)

# ThermalTalk
Pyramid Technologies Thermal Printer API is available on NuGet

    Install-Package ThermalTalk

## v3.0 Changes

Starting with version 3, the SkiaSharp library has replaced System.Drawing to provide cross-platform support.
As a result, ThermalTalk.Imaging classes and functions have received significant modifications
(mainly in `ImageExt.cs` and `PrinterImage.cs`).
Additionally, ThermalTalk and ThermalTalk.Imaging no longer support .NET Framework 4.5.1, instead favoring
.NET Standard 2.0, .NET 5, .NET 6, and .NET 7.
To understand the new changes, check out the ThermalConsole console project.

To provide more flexibility, `ReliancePrinter` and `PhoenixPrinter` can be initialized with an `ISerialConnection`
object for communicating with serial ports. `ThermalUwp` showcases an alternative implementation of `ISerialConnection`
in a Universal Windows Platform (UWP) application, which relies on `Windows.Devices.SerialCommunication` rather than
`System.IO.Ports`.

If you require the old behavior, please see the v2.1.1.0 branch. We'll leave v2.1.1.0 on Nuget but it will not be supported.

## v2.0 Changes

Starting with version 2, we have redesigned the internals of ThermalTalk. This makes printing on Phoenix and Reliance printers
much more reliable for users that utilize many document sections. The key change is that your code must call [FormFeed](https://github.com/PyramidTechnologies/ThermalTalk/blob/master/ThermalTalk/IPrinter.cs#L160) 
in order to print the ticket. This is the function that actually transmits the data to the printer.

If you require the old behavior, please see the v1.4 branch. We'll leave v1.4 on Nuget but it will not be supported.

## Supported Firmware

* Reliance - All Firmware
* Phoenix - S08 Pulse or UNV Firmware in ESC/POS mode

## Samples
See [ThermalConsole](ThermalConsole)

## Supports
* Real Time Status Requests
* Reliance Imaging (Phoenix TBD)
* Basic Font Controls

## Contributing

Please read [CONTRIBUTING.md](CONTRIBUTING.md)

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details
