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
using Microsoft.AspNetCore.Cors;
using CotorraNode.TelemetryComponent.Attributes;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class SGDFLimitsController : Controller
    {
        private readonly Client<SGDFLimits> client;

        public SGDFLimitsController()
        {
            SessionModel.Initialize();
            client = new Client<SGDFLimits>(SessionModel.AuthorizationHeader, clientadapter:
                ClientConfiguration.GetAdapterFromConfig());
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);

            return Json(
                from r in result.AsParallel()
                orderby r.ValidityDate descending
                select new
                {
                    r.ID,
                    r.ValidityDate,
                    r.EG_Especie_GastosMedicos_1,
                    r.EG_Especie_Fija_2,
                    r.EG_Especie_mas_3SMDF_3,
                    r.EG_Prestaciones_en_Dinero_4,
                    r.Invalidez_y_vida_5,
                    r.Cesantia_y_vejez_6,
                    r.Guarderias_7,
                    r.Retiro_8,
                    r.RiesgodeTrabajo_9
                });
        }
    }
}
