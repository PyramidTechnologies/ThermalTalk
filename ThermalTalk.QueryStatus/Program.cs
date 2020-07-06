namespace ThermalTalk.QueryStatus
{
    using System;

    /// <summary>
    /// Simple console app to query the printer status
    /// </summary>
    internal class Program
    {
        public static void Main(string[] args)
        {
            const string reliancePort = "COM12";

            using (var printer = new ReliancePrinter(reliancePort))
            {
                foreach (StatusTypes type in Enum.GetValues(typeof(StatusTypes)))
                {
                    var status = printer.GetStatus(type);

                    // name of status to query
                    Console.WriteLine(Enum.GetName(typeof(StatusTypes), type));
                    
                    // status
                    Console.WriteLine(status.ToJSON());
                }
            }
        }
    }
}