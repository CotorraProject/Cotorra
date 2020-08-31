using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Runtime.Serialization;
using SendGrid.Helpers.Mail.Model;
using System.IO;

namespace Cotorra.Web.Controllers
{
    public class ReportsController : Controller
    {
        public ReportsController()
        {

        }

        [HttpGet]
        //[TelemetryUI]
        public async Task<IActionResult> Index()
        {
            var file = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "reports", "index.html");
            return await Task.FromResult(PhysicalFile(file, "text/html"));
        }
    }
}
