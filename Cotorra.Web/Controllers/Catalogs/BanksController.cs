using System;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using Bank = Cotorra.Web.Models.Bank;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class BanksController : Controller
    {
        private readonly Client<Cotorra.Schema.Bank> client;

        public BanksController()
        {
            SessionModel.Initialize();
            client = new Client<Cotorra.Schema.Bank>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = (await client
                .FindAsync(p => p.Active, SessionModel.CompanyID)).AsParallel()
                .Select(x => new Bank
                {
                    ID = x.ID,
                    Name = x.Name,
                    Description = x.Description,
                    Code = x.Code
                })
                .OrderBy(x => x.Name);

            return Json(result);
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(Bank data)
        {
            throw await Task.FromResult( new Exception("Este catálogo es de sólo lectura"));
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            throw await Task.FromResult( new Exception("Este catálogo es de sólo lectura"));
        }
    }
}
