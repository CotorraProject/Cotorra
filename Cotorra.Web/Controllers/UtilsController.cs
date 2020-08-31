using CotorraNode.TelemetryComponent.Attributes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Net;
using Cotorra.Web.Utils;
using System.Reflection;
using CotorraNode.Common.Config;

namespace Cotorra.Web.Controllers
{
    public class UtilsController : Controller
    {
        private VersionClient _versionClient;

        public UtilsController(VersionClient versionClient)
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
            return Json(_versionClient.GetVersionNumber());
        }

      
    }
}
