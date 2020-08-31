using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.General.Core;
using Cotorra.Schema;

namespace Cotorra.WebAPI.Controllers
{
    /// <summary>
    /// Cotorria Bot Service - Consultas en lenguaje natural
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActionController : BaseCotorraController
    {
         

        [HttpPost("DispatchAsync")]
        public async Task DispatchAsync(DispatchAsyncParams parameters)
        {

            ActionSubscriptionDispatcher actionSubscriptionDispatcher = new ActionSubscriptionDispatcher();

            actionSubscriptionDispatcher.DispatchAsync(parameters.ActionID, parameters.RegisterID);
        }

    }
}
