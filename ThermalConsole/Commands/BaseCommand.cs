using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using ThermalTalk;

namespace ThermalConsole.Commands
{
    public class BaseCommandSettings : CommandSettings
    {
        [CommandArgument(0, "<printerType>")]
        [Description("Either 'reliance' or 'phoenix'.")]
        public string? PrinterType { get; set; }

        [CommandArgument(1, "<portName>")]
        [Description("Serial port name associated with printer.")]
        public string? PortName { get; set; }
    }

    public abstract class BaseCommand<TSettings> : Command<TSettings> where TSettings : BaseCommandSettings
    {
        protected BasePrinter? Printer;

        public override ValidationResult Validate([NotNull] CommandContext context, [NotNull] TSettings settings)
        {
            settings.PrinterType = settings.PrinterType?.ToLower();

            switch (settings.PrinterType)
            {
                case "reliance":
                    Printer = new ReliancePrinter(settings.PortName);
                    break;
                case "phoenix":
                    Printer = new PhoenixPrinter(settings.PortName);
                    break;
                default:
                    return ValidationResult.Error("Printer type must either be 'reliance' or 'phoenix'.");
            }

            var status = Printer.GetStatus(StatusTypes.PrinterStatus);
            return status.IsOnline
                ? ValidationResult.Success()
                : ValidationResult.Error("Printer is not online.");
        }
    }
}