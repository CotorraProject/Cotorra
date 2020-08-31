using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class PeriodDetailsController : Controller
    {
        private readonly Client<PeriodDetail> client;

        public PeriodDetailsController()
        {
            SessionModel.Initialize();
            client = new Client<PeriodDetail>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = (await client
                .GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID)).AsParallel()
                .Select(x => new
                {
                    x.ID,
                    x.Name,
                    x.Description,
                    x.Number,
                    x.PeriodID,
                    x.InitialDate,
                    x.FinalDate,
                    x.PeriodMonth,
                    x.PeriodBimonthlyIMSS,
                    x.PeriodFiscalYear,
                    x.PaymentDays,
                    x.PeriodStatus,
                    x.SeventhDays,
                    x.SeventhDayPosition,
                    FriendlyDesc = $"No. {x.Number} del {x.InitialDate:dd/MM/yy} al {x.FinalDate:dd/MM/yy}"
                });

            return Json(result.OrderBy(p => p.PeriodID).ThenBy(p=> p.Number));
        }
         
    }
}
