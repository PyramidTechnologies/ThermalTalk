using Spectre.Console;
using Spectre.Console.Cli;
using ThermalConsole.Commands;

AnsiConsole.Write(new FigletText("Thermal Console"));
AnsiConsole.WriteLine();

// Build the app.
var app = new CommandApp();

// Add commands.
app.Configure(config =>
{
    config.AddCommand<QueryCommand>("query")
        .WithDescription("Query the printer for its status.")
        .WithExample("query", "reliance", "COM11")
        .WithExample("query", "phoenix", "COM10");

    config.AddCommand<PrintBarcodeCommand>("printcode")
        .WithDescription("Print a barcode based on given text.")
        .WithExample("printcode", "reliance", "COM11", "Forever by your side.")
        .WithExample("printcode", "phoenix", "COM10", "Rise from the ashes.");

    config.AddCommand<CaptureCommand>("capture")
        .WithDescription("Capture an image from the 1st available webcam and print it.")
        .WithExample("capture", "reliance", "COM11")
        .WithExample("capture", "phoenix", "COM10");
});

// Modify arguments here if desired.
// args = new[] { "query", "reliance", "COM11" };
// args = new[]
// {
//     "printcode", "reliance", "COM11",
//     "Lorem ipsum dolor sit amet, consectetuer adipiscing elit. Aenean commodo ligula eget dolor. Aenean massa. Cum sociis natoque penatibus et magnis dis parturient montes, nascetur ridiculus mus. Donec quam felis, ultricies nec, pellentesque eu, pretium quis, sem. Nulla consequat massa quis enim. Donec pede justo, fringilla vel, aliquet nec, vulputate eget, arcu. In enim justo, rhoncus ut, imperdiet a, venenatis vitae, justo. Nullam dictum felis eu pede mollis pretium. Integer tincidunt. Cras dapibu"
// };
// args = new[] { "capture", "reliance", "COM11" };

// Execute the app.
return app.Run(args);