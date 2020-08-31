using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Schema;
using System.Linq;
using System.IO;
using System.Reflection;
using Cotorra.Schema.Report;
using Cotorra.Core.Reports;
using CotorraNode.App.Common.UX;
using System.Runtime.Serialization;
using CotorraNode.Common.Config;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cotorra.Web.Controllers
{
    public class ReportController : Controller
    {
        protected readonly string _stimulsoftKey;
        private static Dictionary<string, string> _dictionaryMRT = new Dictionary<string, string>();
        private readonly Dictionary<string, ReportCatalog> _dictionaryReportCatalog = new Dictionary<string, ReportCatalog>();
        private const string CONTENT_RESULT = "text/xml";

        public ReportController()
        {
            _stimulsoftKey = ConfigManager.GetValue("StimulsoftKey");
            _dictionaryReportCatalog.Add("RayaReportParams", ReportCatalog.Raya);
            _dictionaryReportCatalog.Add("FonacotReportParams", ReportCatalog.Fonacot);
        }

        [HttpGet]
        public async Task<ActionResult> GetByName(string name)
        {
            //var fileContent = String.Empty;
            //if (_dictionaryMRT.TryGetValue(name, out fileContent))
            //{
            //    return Content(fileContent, CONTENT_RESULT);
            //}
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", "src", name + ".mrt");
            var report = await System.IO.File.ReadAllTextAsync(file);
            //_dictionaryMRT.TryAdd(name, report);
            return Content(report, CONTENT_RESULT);
        }

        [Cotorra.Web.Utils.TelemetryUI]
        [Authentication]
        [HttpGet]
        public async Task<ActionResult> GetData(string param, string typename)
        {
            try
            {
                var instanceID = SessionModel.InstanceID;
                if (instanceID == Guid.Empty)
                {
                    SessionModel.Initialize();
                    instanceID = SessionModel.InstanceID;
                }
                var type = GetType(typename);
                var parameters = (IReportParams)JsonConvert.DeserializeObject(param, type);
                parameters.InstanceID = instanceID;
                parameters.company = SessionModel.CompanyID;
                parameters.user = SessionModel.IdentityID;
                var jsonData = await GetDataReport(parameters, typename);

                //Result
                var reportData = new ReportData()
                {
                    Data = jsonData,
                    Key = _stimulsoftKey,
                };

                return Json(reportData);
            }
            catch 
            {
                throw;
            }
        }

        private async Task<String> GetDataReport(IReportParams parameters, string typename)
        {
            ReportManager rportManager = new ReportManager();
            _dictionaryReportCatalog.TryGetValue(typename, out ReportCatalog reportCatalog);
            return (await rportManager.ProcessAsync(
                new ReportManagerParams()
                {
                    ReportCatalog = reportCatalog,
                    ReportParams = parameters
                }))
                .ReportResultDetails.FirstOrDefault().JsonResult;
        }

        private Type GetType(string name)
        {
            return Assembly.GetAssembly(typeof(Employee)).GetType("Cotorra.Schema.Report." + name);
        }
    }

    [DataContract]
    public class ReportData
    {
        [DataMember]
        public string Data { get; set; }

        [DataMember]
        public string Key { get; set; }
    }
}
