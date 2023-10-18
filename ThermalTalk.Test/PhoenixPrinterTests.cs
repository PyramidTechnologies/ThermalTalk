namespace ThermalTalk.Test
{
    using NUnit.Framework;

    
    [TestFixture]
    public class PhoenixPrinterTests
    {
        // Change this to connected printer's port.
        private const string TEST_PORT = "COM1";

        [Test]
        public void PhoenixCtorTest()
        {
            var printer = new PhoenixPrinter("TEST");
            Assert.IsNotNull(printer);
        }

        [Test()]
        public void GetStatusTest_Status()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new PhoenixPrinter(TEST_PORT);

            var status = printer.GetStatus(StatusTypes.PrinterStatus);

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
            var printer = new PhoenixPrinter(TEST_PORT);

            var status = printer.GetStatus(StatusTypes.OfflineStatus);

            Assert.IsNotNull(status);

            // Only these should be set
            Assert.IsNotNull(status.HasError);

            // All the rest must be null
            Assert.IsNull(status.IsCoverClosed);
            Assert.IsNull(status.IsNormalFeed);
            Assert.IsNull(status.IsPaperPresent);
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
            var printer = new PhoenixPrinter(TEST_PORT);

            // TODO base ESC/POS firmware does not support this
            return;
            var status = printer.GetStatus(StatusTypes.ErrorStatus);

            // If this fails, we have a hardware or configuration issue
            Assert.IsFalse(status.IsInvalidReport);

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
            var printer = new PhoenixPrinter(TEST_PORT);

            var status = printer.GetStatus(StatusTypes.PaperStatus);

            // If this fails, we have a hardware or configuration issue
            Assert.IsFalse(status.IsInvalidReport);

            Assert.IsNotNull(status);
            
            // Only these should be set
            Assert.IsNotNull(status.IsPaperPresent);

            // All the rest must be null
            Assert.IsNull(status.IsPaperLevelOkay);
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
        public void Print2DBarcodeTest()
        {
            var printer = new PhoenixPrinter(TEST_PORT);

            var status = printer.GetStatus(StatusTypes.PaperStatus);


            printer.Print2DBarcode("Hello World");
        }

        [Test]
        public void LoggerTest()
        {
            var printer = new PhoenixPrinter(TEST_PORT)
            {
                Logger = new TestLogger()
            };

            if (printer.Logger is TestLogger tl)
            {
                Assert.IsEmpty(tl.TraceOutput);

                printer.PrintNewline();
                
                Assert.IsNotEmpty(tl.TraceOutput);
            }
            
            
        }
    }
}
