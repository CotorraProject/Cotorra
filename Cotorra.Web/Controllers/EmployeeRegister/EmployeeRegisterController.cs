using System;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq.Expressions;
using System.Linq;
using CotorraNode.TelemetryComponent.Attributes;
using Cotorra.Web.Utils;
using System.IO;

namespace Cotorra.Web.Controllers
{
    public class EmployeeRegisterController : Controller
    {
        public EmployeeRegisterController()
        {

        }


        [HttpGet]
        [TelemetryUI]
        [Route("/finalizarregistro")]
        public async Task<ContentResult> Index()
        {
            String html = "";
            using (StreamReader streamReader = new StreamReader("wwwroot/views/employeeregister/index.html"))
            {
                html = await streamReader.ReadToEndAsync();
            }

            return new ContentResult
            {
                ContentType = "text/html",
                Content = html
            };
        }

    }
}
