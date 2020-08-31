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

    public class FonacotReport : BaseReport, IReport
    {
        #region "Attributes"

        protected const int PAYROLLCONFIGURATION_POSITIONTABLE = 0;
        protected const int FONACOT_EMPLOYEE_POSITIONTABLE = 1;
        protected const int FONACOT_CREDITS_POSITIONTABLE = 2; 

        protected const string REPORT_SP_NAME = "Report_Fonacot";
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
            reportResult.ReportResultDetails = new List<ReportResultDetail>();

            //convert to RayaParams from Interface
            var localReportParams = reportParams as FonacotReportParams;

            try
            {
                //Process Raya Report
                var parameters = new Dictionary<string, object>
                {
                    { "@company", localReportParams.company },
                    { "@InstanceId", localReportParams.InstanceID },
                    { "@user", localReportParams.user },
                    { "@InitialYear", localReportParams.InitialFiscalYear },
                    { "@InitialMonth", localReportParams.InitialMonth },
                    { "@FinalYear", localReportParams.FinalFiscalYear },
                    { "@FinalMonth", localReportParams.FinalMonth },
                    { "@CreditStatus", localReportParams.CreditStatus },
                    { "@EmployeeStatus", localReportParams.EmployeeStatus },
                    { "@EmployeeFilter", localReportParams.EmployeeFilter }
                };


                //Execute SP
                DataSet ds = await base.ExecuteSPAsync(REPORT_SP_NAME, parameters);

                //Fill json results
                dynamic expando = new ExpandoObject();
                expando.Header = base.GetObjectFromDataTable(ds, PAYROLLCONFIGURATION_POSITIONTABLE);
                expando.Employees = base.GetObjectFromDataTable(ds, FONACOT_EMPLOYEE_POSITIONTABLE);
                expando.Credits = base.GetObjectFromDataTable(ds, FONACOT_CREDITS_POSITIONTABLE); 

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


            return await Task.FromResult(reportResult);
        }
    }


}
