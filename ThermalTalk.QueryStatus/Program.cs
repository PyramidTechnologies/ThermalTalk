using System.Linq;

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
            var reliancePort = args.FirstOrDefault();

            Console.WriteLine($"The following port name was selected: {reliancePort}");

            if (reliancePort is null)
            {
                Console.WriteLine("No port was specified. Please specify the port your reliance printer is on.");
                return;
            }
            
            try
            {
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
            catch (Exception e)
            {
                Console.WriteLine($"An exception was caught. Did you specify a valid port name? \n {e}");
            }
        }
    }
}