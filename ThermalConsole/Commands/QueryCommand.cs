using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Diagnostics.CodeAnalysis;
using ThermalTalk;

namespace ThermalConsole.Commands
{
    public class QueryCommand : BaseCommand<BaseCommandSettings>
    {
        public override int Execute([NotNull] CommandContext context, [NotNull] BaseCommandSettings settings)
        {
            foreach (StatusTypes type in Enum.GetValues(typeof(StatusTypes)))
            {
                var status = Printer!.GetStatus(type);
                AnsiConsole.MarkupLine($"[green]{Enum.GetName(typeof(StatusTypes), type)}[/]");
                AnsiConsole.MarkupLine($"[green]{status.ToJSON()}[/]\n");
            }

            return 0;
        }
    }
}