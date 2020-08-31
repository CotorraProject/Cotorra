using System.Threading.Tasks;
using CotorraNode.Common.Config;
using Microsoft.AspNetCore.Mvc;

namespace Cotorra.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : BaseCotorraController
    {
        [HttpGet]
        public async Task<ActionResult<string>> Get()
        {
            var buildNumber = await Task.FromResult(ConfigManager.GetValue("buildNumber"));
            return new JsonResult(buildNumber);
        }
    }
}
