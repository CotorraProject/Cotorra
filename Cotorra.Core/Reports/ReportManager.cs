using Cotorra.Schema.Report;
using System.Threading.Tasks;

namespace Cotorra.Core.Reports
{
    public class ReportManager
    {
        public async Task<ReportResult> ProcessAsync(ReportManagerParams reportManagerParams)
        {
            var reportInstance = ReportFactory.CreateInstance(reportManagerParams.ReportCatalog);
            var result = await reportInstance.ProcessAsync(reportManagerParams.ReportParams);

            return result;
        }      
    }
}
