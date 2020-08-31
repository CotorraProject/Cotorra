using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class UMIController : Controller
    {
        private readonly Client<UMI> client;

        public UMIController()
        {
            SessionModel.Initialize();
            client = new Client<UMI>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync( );

            return Json(
                from r in result
                orderby r.ValidityDate descending
                select new
                {
                    r.ID,
                    InitialDate = r.ValidityDate,
                    r.Value
                });
        }
    }
}
