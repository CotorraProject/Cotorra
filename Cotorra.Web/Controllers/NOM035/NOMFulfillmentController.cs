using System;
using Microsoft.AspNetCore.Mvc;
using System.Dynamic;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    public class NOMFulfillmentController : Controller
    {
        public NOMFulfillmentController()
        {
        }

        [HttpGet]
        [TelemetryUI]
        [ResponseCache(Duration = 2600)]
        public async Task<JsonResult> Get()
        {
            return await Task.FromResult( Json("OK"));
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetDetail(Guid periodID, Guid surveyID)
        {
            dynamic result = new ExpandoObject();
            result.Employees = new List<Object>()
            {
                new { Name = "Murillo Rivera Louis Shaid Yamani", EvaluationStatus = 4 },
                new { Name = "Tejeda Sánchez Luis Alfredo", EvaluationStatus = 1 },
                new { Name = "Ramírez Méndez Héctor Omar", EvaluationStatus = 2 },
                new { Name = "Nuñez Basurto Jose Alberto", EvaluationStatus = 3 },
                new { Name = "Estrada Estrada Luis Eduardo", EvaluationStatus = 4 },
                new { Name = "Gonzalez Gonzalez Cesar", EvaluationStatus = 1 },
            };

            return await Task.FromResult(Json(result));
        }
    }
}
