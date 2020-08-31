using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers.Help
{
    public class GuideHelpController : Controller
    {

        public GuideHelpController()
        {
            SessionModel.Initialize();
        }

        private static List<GuidHelp> guideHelp = new List<GuidHelp>()
        {
            new GuidHelp() { ID = Guid.NewGuid(), ExternalID = 32148, Name = "Espacio de trabajo", Context = "all" },
            new GuidHelp() { ID = Guid.NewGuid(), ExternalID = 64323, Name = "Conoce tu dashboard", Context = "home" }
        };

        // GET: GuideHelp
        [CotorraNode.TelemetryComponent.Attributes.Telemetry]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public async Task<ActionResult> GetHelp(String context)
        {
            JsonResult result = new JsonResult("");
            await Task.Factory.StartNew(() =>
            {
                try
                {
                    result = Json(guideHelp.Where(p => p.Context.Contains(context) || p.Context.Contains("all")));
                }
                catch { result = Json("Error, no se encontró tutoriales para este menú"); }
            });

            return result;
        }
    }

    //TODO: Delete this class is just for example
    class GuidHelp
    {
        public Guid ID { get; set; }
        public Int32 ExternalID { get; set; }
        public String Name { get; set; }

        public String Context { get; set; }

    }
}
