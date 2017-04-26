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
using NUnit.Framework;
namespace ThermalTalk.Tests
{
    [TestFixture()]
    public class ReliancePrinterTests
    {
        private const string TEST_PORT = "COM1";

        [Test()]
        public void ReliancePrinterTest()
        {
            // Test ctor
            var printer = new ReliancePrinter(TEST_PORT);
            Assert.IsNotNull(printer);
        }
        
        [Test()]
        public void GetStatusTest_Status()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.Status);

            Assert.IsNotNull(status);

            // Only IsOnline should be set
            Assert.IsNotNull(status.IsOnline);

            // All the rest must be null
            Assert.IsNull(status.IsPaperPresent);
            Assert.IsNull(status.IsPaperLevelOkay);
            Assert.IsNull(status.IsTicketPresentAtOutput);
            Assert.IsNull(status.IsCoverClosed);
            Assert.IsNull(status.IsPaperMotorOff);
            Assert.IsNull(status.IsDiagButtonReleased);
            Assert.IsNull(status.IsHeadTemperatureOkay);
            Assert.IsNull(status.IsCommsOkay);
            Assert.IsNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNull(status.IsPaperPathClear);
            Assert.IsNull(status.IsCutterOkay);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.HasError);
            Assert.IsNull(status.HasFatalError);
            Assert.IsNull(status.HasRecoverableError);

        }

        [Test()]
        public void GetStatusTest_OffLineStatus()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.OffLineStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.IsCoverClosed);
            Assert.IsNotNull(status.IsNormalFeed);
            Assert.IsNotNull(status.IsPaperPresent);
            Assert.IsNotNull(status.HasError);

            // All the rest must be null
            Assert.IsNull(status.IsOnline);
            Assert.IsNull(status.IsPaperLevelOkay);
            Assert.IsNull(status.IsTicketPresentAtOutput);
            Assert.IsNull(status.IsPaperMotorOff);
            Assert.IsNull(status.IsDiagButtonReleased);
            Assert.IsNull(status.IsHeadTemperatureOkay);
            Assert.IsNull(status.IsCommsOkay);
            Assert.IsNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNull(status.IsPaperPathClear);
            Assert.IsNull(status.IsCutterOkay);
            Assert.IsNull(status.HasFatalError);
            Assert.IsNull(status.HasRecoverableError);


        }

        [Test()]
        public void GetStatusTest_ErrorStatus()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.ErrorStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.IsCutterOkay);
            Assert.IsNotNull(status.HasFatalError);
            Assert.IsNotNull(status.HasRecoverableError);

            // All the rest must be null
            Assert.IsNull(status.IsOnline);
            Assert.IsNull(status.IsPaperPresent);
            Assert.IsNull(status.IsPaperLevelOkay);
            Assert.IsNull(status.IsTicketPresentAtOutput);
            Assert.IsNull(status.IsCoverClosed);
            Assert.IsNull(status.IsPaperMotorOff);
            Assert.IsNull(status.IsDiagButtonReleased);
            Assert.IsNull(status.IsHeadTemperatureOkay);
            Assert.IsNull(status.IsCommsOkay);
            Assert.IsNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNull(status.IsPaperPathClear);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.HasError);


                    }

        [Test()]
        public void GetStatusTest_PaperRollStatus()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.PaperRollStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.IsPaperLevelOkay);
            Assert.IsNotNull(status.IsPaperPresent);

            // All the rest must be null
            Assert.IsNull(status.IsOnline);
            Assert.IsNull(status.IsTicketPresentAtOutput);
            Assert.IsNull(status.IsCoverClosed);
            Assert.IsNull(status.IsPaperMotorOff);
            Assert.IsNull(status.IsDiagButtonReleased);
            Assert.IsNull(status.IsHeadTemperatureOkay);
            Assert.IsNull(status.IsCommsOkay);
            Assert.IsNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNull(status.IsPaperPathClear);
            Assert.IsNull(status.IsCutterOkay);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.HasError);
            Assert.IsNull(status.HasFatalError);
            Assert.IsNull(status.HasRecoverableError);

        }

        [Test()]
        public void GetStatusTest_PrintStatus()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.PrintStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.IsPaperMotorOff);
            Assert.IsNotNull(status.IsPaperPresent);

            // All the rest must be null
            Assert.IsNull(status.IsOnline);
            Assert.IsNull(status.IsPaperLevelOkay);
            Assert.IsNull(status.IsTicketPresentAtOutput);
            Assert.IsNull(status.IsCoverClosed);
            Assert.IsNull(status.IsDiagButtonReleased);
            Assert.IsNull(status.IsHeadTemperatureOkay);
            Assert.IsNull(status.IsCommsOkay);
            Assert.IsNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNull(status.IsPaperPathClear);
            Assert.IsNull(status.IsCutterOkay);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.HasError);
            Assert.IsNull(status.HasFatalError);
            Assert.IsNull(status.HasRecoverableError);

                    }

        [Test()]
        public void GetStatusTest_FullStatus()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter(TEST_PORT);

            var status = printer.GetStatus(StatusRequests.FullStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.IsPaperPresent);
            Assert.IsNotNull(status.IsPaperLevelOkay);
            Assert.IsNotNull(status.IsTicketPresentAtOutput);
            Assert.IsNotNull(status.IsCoverClosed);
            Assert.IsNotNull(status.IsPaperMotorOff);
            Assert.IsNotNull(status.IsDiagButtonReleased);
            Assert.IsNotNull(status.IsHeadTemperatureOkay);
            Assert.IsNotNull(status.IsCommsOkay);
            Assert.IsNotNull(status.IsPowerSupplyVoltageOkay);
            Assert.IsNotNull(status.IsPaperPathClear);
            Assert.IsNotNull(status.IsCutterOkay);

            // All the rest must be null
            Assert.IsNull(status.IsOnline);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.HasError);
            Assert.IsNull(status.HasFatalError);
            Assert.IsNull(status.HasRecoverableError);

        }
    }
}
