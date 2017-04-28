#region Copyright & License
/*
MIT License

Copyright (c) 2017 Pyramid Technologies

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
 */
#endregion
namespace ThermalTalk
{
    using Microsoft.Win32;
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Text.RegularExpressions;

    internal static class SerialPortUtils
    {
        private const string REG_COM_STRING = @"HARDWARE\DEVICEMAP\SERIALCOMM";

        /// <summary>
        /// Performs a best effort guess if the specfied port is a VCP. This 
        /// performs a series of registry checks to determine if this port
        /// number is currently associated with a known VCP driver.
        /// </summary>
        /// <param name="portName">Name of port to investigate</param>
        /// <returns>True if port was last associated with VCP</returns>
        internal static bool IsVirtualComPort(string portName)
        {
            return GetPortByVIDPID("", "", true).Contains(portName);
        }

        /// <summary>
        /// Compile an array of COM port names associated with given VID and PID
        /// </summary>
        /// <param name="VID">string representing the vendor id of the USB/Serial convertor</param>
        /// <param name="PID">string representing the product id of the USB/Serial convertor</param>
        /// <param name="any">Set to true to ignore VID/PID and return all known ports</param>
        /// <returns></returns>
        internal static IEnumerable<string> GetPortByVIDPID(string VID, string PID, bool any = false)
        {
            var pattern = String.Format("^VID_{0}.PID_{1}", VID, PID);
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var comports = new List<string>();

            using(var rk1 = Registry.LocalMachine)
            using(var rk2 = rk1.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB"))
            {

                // Open top-level device ID
                foreach (var s3 in rk2.GetSubKeyNames())
                {

                    // Open unique device instance.
                    var rk3 = rk2.OpenSubKey(s3);                    
                    foreach (var s in rk3.GetSubKeyNames())
                    {
                        // Does this match the specified VID/PID or did user say I want them all?
                        if (regex.Match(s).Success || any)
                        {
                            // Only look at targets with the Class value set to Ports
                            var rk4 = rk3.OpenSubKey(s);
                            var classVal = Array.Find(rk4.GetValueNames(), (x) => x.Equals("Class"));
                            if(string.IsNullOrEmpty(classVal))
                            {
                                continue;
                            }

                            if(!rk4.GetValue(classVal, string.Empty).Equals("Ports"))
                            {
                                continue;
                            }

                            // Make sure it has a proper Device Parameters value
                            var devParams = Array.Find(rk4.GetSubKeyNames(), (x) => x.Equals("Device Parameters"));
                            if (string.IsNullOrEmpty(devParams))
                            {
                                continue;
                            }

                            // Extract that port name
                            var rk5 = rk4.OpenSubKey(devParams);
                            comports.Add((string)rk5.GetValue("PortName", string.Empty));

                            rk5.Close();                            
                            rk4.Close();                                
                        }
                    }

                    rk3.Close();                    
                }

                // Return unique, non-empty port names
                return comports.Distinct().Where(x => !string.IsNullOrEmpty(x));
            }
        }
    }
}
