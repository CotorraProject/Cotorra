using Cotorra.Schema.Report;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Reports
{
    public static class ReportFactory
    {
        public static IReport CreateInstance(ReportCatalog reportCatalog)
        {
            if (reportCatalog == ReportCatalog.Raya)
            {
                return new RayaReport();
            }
            else if(reportCatalog == ReportCatalog.Fonacot)
            {
                return new FonacotReport();
            }

            return new RayaReport();
        }
    }
}
