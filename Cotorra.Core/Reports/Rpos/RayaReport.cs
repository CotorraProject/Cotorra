using Newtonsoft.Json;
using Cotorra.Schema;
using Cotorra.Schema.Report;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Dynamic;
using System.Threading.Tasks;

namespace Cotorra.Core.Reports
{
    /// <summary>
    /// Raya Reports
    /// </summary>
    public class RayaReport : BaseReport, IReport
    {
        #region "Attributes"
        protected const int PAYROLLCONFIGURATION_POSITIONTABLE = 0;
        protected const int RAYA_EMPLOYEE_POSITIONTABLE = 1;
        protected const int RAYA_DETAILS_OVERDRAFT_POSITIONTABLE = 2;
        protected const int RAYA_DETAILS_CONCEPTGLOBALS_POSITIONTABLE = 3;
        protected const int RAYA_DETAILS_IMSS_POSITIONTABLE = 4;
        protected const string REPORT_SP_NAME = "Report_Raya";
        #endregion

        /// <summary>
        /// Process raya report
        /// </summary>
        /// <param name="reportParams"></param>
        /// <returns></returns>
        public async Task<ReportResult> ProcessAsync(IReportParams reportParams)
        {
            //result 
            var reportResult = new ReportResult();

            //convert to RayaParams from Interface
            var rayaReportParams = reportParams as RayaReportParams;

            try
            {
                //Process Raya Report
                var parameters = new Dictionary<string, object>();
                parameters.Add("@company", rayaReportParams.company);
                parameters.Add("@InstanceId", rayaReportParams.InstanceID);
                parameters.Add("@user", rayaReportParams.user);
                parameters.Add("@EmployerRegistrationID", rayaReportParams.EmployerRegistrationID);
                parameters.Add("@PeriodDetailID", rayaReportParams.PeriodDetailID);

                //Execute SP
                DataSet ds = await base.ExecuteSPAsync(REPORT_SP_NAME, parameters);

                //Fill json results
                dynamic expando = new ExpandoObject();
                expando.Header = base.GetObjectFromDataTable(ds, PAYROLLCONFIGURATION_POSITIONTABLE);
                expando.Employees = base.GetObjectFromDataTable(ds, RAYA_EMPLOYEE_POSITIONTABLE);
                expando.OverdraftDetails = base.GetObjectFromDataTable(ds, RAYA_DETAILS_OVERDRAFT_POSITIONTABLE);
                expando.ConceptGlobals = base.GetObjectFromDataTable(ds, RAYA_DETAILS_CONCEPTGLOBALS_POSITIONTABLE);
                expando.IMSSDetails = base.GetObjectFromDataTable(ds, RAYA_DETAILS_IMSS_POSITIONTABLE);

                //Expando
                var jResult = JsonConvert.SerializeObject(expando);
                reportResult.ReportResultDetails.Add(new ReportResultDetail(REPORT_SP_NAME, jResult));

                //Limpiamos el Dataset
                ds.Clear();
            }
            catch (Exception ex)
            {
                throw new CotorraException(201, "201", $"Ocurrió un error al generar el reporte de raya: {ex.Message}", ex);
            }

            return reportResult;
        }
    }
}
