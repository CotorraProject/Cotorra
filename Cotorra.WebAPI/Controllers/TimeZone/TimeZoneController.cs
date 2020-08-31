using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using TimeZoneConverter;

namespace Cotorra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeZoneController
    {
        [HttpGet("GetTimeZoneByZipCode/{zipCode}")]   
        public async Task<ActionResult<string>> GetTimeZoneByZipCode(string zipCode)
        {
            //Obtener los zipCodes
            var zipCodeMiddlewareManager = new MiddlewareManager<catCFDI_CodigoPostal>(
                new BaseRecordManager<catCFDI_CodigoPostal>(),
               new catCFDI_CodigoPostalValidator());
            var zipCodes = await zipCodeMiddlewareManager.FindByExpressionAsync(p =>
                    p.c_CodigoPostal == zipCode
                , Guid.Empty);

            //Zip Code manager
            Dictionary<string, string> timeZones = new Dictionary<string, string>();
            timeZones.Add("Tiempo del Centro", "Central Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico", "Pacific Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico en Frontera", "Pacific Standard Time");
            timeZones.Add("Tiempo del Noroeste", "Mountain Standard Time (Mexico)");
            timeZones.Add("Tiempo del Pacífico Sonora", "Pacific Standard Time (Mexico)");
            timeZones.Add("Tiempo del Centro en Frontera", "Central Standard Time");
            timeZones.Add("Tiempo del Noroeste en Frontera", "Pacific Standard Time");
            timeZones.Add("Tiempo del Sureste", "Eastern Standard Time");

            timeZones.TryGetValue(zipCodes.FirstOrDefault().DescripcionHusoHorario, out string timeZone);
            TimeZoneInfo tzi = TZConvert.GetTimeZoneInfo(timeZone);
            var dateTimeNow = DateTime.Now;
            var result = TimeZoneInfo.ConvertTime(dateTimeNow, tzi);

            return new JsonResult(new { timezone = timeZone, datetime = result });
        }
    }
}
