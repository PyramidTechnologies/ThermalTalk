using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RelianceTalk;
using NUnit.Framework;
namespace RelianceTalk.Tests
{
    [TestFixture()]
    public class ReliancePrinterTests
    {

        [Test()]
        public void ReliancePrinterTest()
        {
            // Test ctor
            var printer = new ReliancePrinter("test");
            Assert.IsNotNull(printer);

        }
        
        [Test()]
        public void GetStatusBidiTest()
        {
            var printer = new ReliancePrinter("Reliance");
            var status = printer.GetStatus(StatusRequests.FullStatus);

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

        [Test()]
        public void GetStatusTest()
        {
            // Tests may run in parallel and since we are using a serial port
            // we should just run all queries in one test to avoid access issues.
            var printer = new ReliancePrinter("Reliance", "COM25");




            var status = printer.GetStatus(StatusRequests.Status);

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




            status = printer.GetStatus(StatusRequests.OffLineStatus);

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




            status = printer.GetStatus(StatusRequests.ErrorStatus);

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



            status = printer.GetStatus(StatusRequests.PaperRollStatus);

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



            status = printer.GetStatus(StatusRequests.PrintStatus);

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



            status = printer.GetStatus(StatusRequests.FullStatus);

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
