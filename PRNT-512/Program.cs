// Create document

using ThermalTalk;

using var printer = new ReliancePrinter("COM3");

var document = new StandardDocument
{
    CodePage = CodePages.CP1252,
};

// create and add barcode
var barcode = new TwoDBarcode(TwoDBarcode.Flavor.Reliance)
{
    EncodeThis = "1FBxB5atGb4hLpfRDD7h8cnAH1unU9HT7i"
};

document.Sections.Add(new BarcodeSection(barcode));

// add in text subsequent to barcode
document.Sections.Add(new StandardSection
{
    Content = "1FBxB5atGb4hLpfRDD7h8cnAH1unU9HT7i",
});

printer.PrintDocument(document);
printer.FormFeed();