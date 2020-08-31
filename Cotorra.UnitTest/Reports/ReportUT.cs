using Cotorra.Core.Reports;
using Cotorra.Schema.Report;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.IO;
using System.Linq;
using Cotorra.Core.Utils;
using Cotorra.Schema;

namespace Cotorra.UnitTest.Reports
{
    public class ReportUT
    {
        [Fact]
        public async Task RayaReportCreation()
        {
            var rayaReportParams = new RayaReportParams()
            {
                company = Guid.Parse("0C08DAA6-F775-42A8-B75E-1B9B685B7977"),
                InstanceID = Guid.Parse("A9C56B65-3E23-4256-AC9C-4FF0F6979CD2"),
                user = Guid.NewGuid(),
                EmployerRegistrationID = Guid.Parse("B76B0814-6B92-49DC-9038-00BB31B0A141"),
                PeriodDetailID = Guid.Parse("3E028190-C1C1-400E-B807-0331C862BA34")
            };

            var rayaReport = new RayaReport();
            var reportResult = await rayaReport.ProcessAsync(rayaReportParams);

            var pathFile = Path.Combine(DirectoryUtil.AssemblyDirectory, "raya_report.json");
            await File.WriteAllTextAsync(pathFile, reportResult.ReportResultDetails.FirstOrDefault().JsonResult);
        }

        [Fact]
        public async Task FonacotReportCreation()
        {
            var fonacotReportParams = new FonacotReportParams()
            {
                company = Guid.Parse("FB0EE870-39C5-40A2-B6C5-9C360D306804"),
                InstanceID = Guid.Parse("0645A10F-8B2F-4996-81F1-954BC20308AB"),
                user = Guid.NewGuid(),
                CreditStatus = (int)FonacotMovementStatus.Active,
                EmployeeFilter = null,
                EmployeeStatus = (int)CotorriaStatus.Active,
                InitialFiscalYear = 2020,
                FinalFiscalYear = 2020,
                FinalMonth = 3,
                InitialMonth = 1
            };

            var fonacotReport = new FonacotReport();
            var reportResult = await fonacotReport.ProcessAsync(fonacotReportParams);

            var pathFile = Path.Combine(DirectoryUtil.AssemblyDirectory, "fonacot_report.json");
            await File.WriteAllTextAsync(pathFile, reportResult.ReportResultDetails.FirstOrDefault().JsonResult);
        }
    }
}
