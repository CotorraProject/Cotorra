using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using Cotorra.Core;

namespace Cotorra.Web.Controllers
{

    [Authentication]
    public class BankPaymentLayoutController : Controller
    {
        private readonly BankPaymentLayoutClient client;

        public BankPaymentLayoutController()
        {
            SessionModel.Initialize();
            client = new BankPaymentLayoutClient(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid employeeID)
        {
            return await Task.FromResult(Json("OK"));
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> GenerateLayout(BankLayoutPaymentInformation layoutInformation)
        {
            var parameters = layoutInformation;

            parameters.IdentityWorkID = SessionModel.CompanyID;
            parameters.InstanceID = SessionModel.InstanceID;
            parameters.TokeUser = SessionModel.AuthorizationHeader;

            var urlLayout = await client.GenerateBankLayoutPeriod(parameters);

            return Json(urlLayout);
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            return await Task.FromResult(Json("OK"));
        }
    }
}