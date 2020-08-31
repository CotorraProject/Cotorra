using Cotorra.Schema.Report;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Schema.Report
{
    public interface IReport
    {
        public Task<ReportResult> ProcessAsync(IReportParams reportParams);
    }
}
