using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using Cotorra.Web.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;

namespace Cotorra.Web.Controllers
{
    public class HomeController : Controller
    {
        private VersionClient _versionClient;

        public HomeController(VersionClient versionClient)
        {
            _versionClient = versionClient;            
        }

        [HttpGet]
        [TelemetryUI]
        public JsonResult Index()
        {
            return Json("OK");
        }

        [HttpGet]
        [TelemetryUI]
        public JsonResult GetVersionExecutionTime()
        {            
            return Json($"{_versionClient.GetVersionNumber()} OS:{VersionUtil.GetUserOS()}");
        }

        [HttpGet]
        public JsonResult InitSession()
        {
            CotorraTools.MemoryCache = new MemoryCache(new MemoryCacheOptions());
            return Json(_versionClient.GetVersionNumber());
        }

        [HttpGet]
        [TelemetryUI]
        public JsonResult GetZipCodeInfo(String zipCode)
        {
            HttpWebRequest myRequest = (HttpWebRequest)WebRequest.Create("https://micodigopostal.org/buscarcp.php?buscar=" + zipCode);
            myRequest.Method = "GET";
            WebResponse myResponse = myRequest.GetResponse();
            StreamReader sr = new StreamReader(myResponse.GetResponseStream(), System.Text.Encoding.UTF8);
            String result = sr.ReadToEnd();
            sr.Close();
            myResponse.Close();

            return Json(result);
        }

        [HttpGet]
        public async Task<JsonResult> SendMailTest()
        {
            var apiKey = "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"; //Environment.GetEnvironmentVariable("NAME_OF_THE_ENVIRONMENT_VARIABLE_FOR_YOUR_SENDGRID_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress("notificaciones@mail.Cotorria.mx", "Cotorria");
            var subject = "Envío de comprobante de nómina";
            var to = new EmailAddress("yamani.murillo@gmail.com", "Yamani Murillo");
            var plainTextContent = "Aquí tienes tu recibo de nómina perteneciente al periodo 15";
            var htmlContent = "<strong>Aquí tienes tu recibo de nómina perteneciente al periodo 15</strong>";
            var msg = MailHelper.CreateSingleTemplateEmail(from, to, "d-c9cd26e940214ee09967f1392591ae76", new { Subject = "Subject " + Guid.NewGuid().ToString(), EmployeeName = "Yamani Murillo" });
            var response = await client.SendEmailAsync(msg);
            return Json("OK");
        }
    }
}
