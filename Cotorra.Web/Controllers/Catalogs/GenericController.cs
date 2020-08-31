using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class GenericController : Controller
    {
        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var response = new Object();
            await Task.Run(() =>
            {
                //Get
            });

            return Json(response);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Object data)
        {
            Guid? resultID = new Guid();

            await Task.Run(() =>
            {
               //Save-Update
            });

            return Json(resultID);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await Task.Run(() =>
            {

            });

            return Json("OK");
        }
    }
}
