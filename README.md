[![NuGet](https://img.shields.io/nuget/v/ThermalTalk.svg)](https://www.nuget.org/packages/ThermalTalk/)

# ThermalTalk
Pyramid Technologies Thermal Printer API is available on NuGet

    Install-Package ThermalTalk

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
