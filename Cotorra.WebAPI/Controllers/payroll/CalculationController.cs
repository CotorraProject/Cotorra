using AutoMapper;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Schema;
using Cotorra.WebAPI.ActionFilters;
using MoreLinq;
using System.Linq;

namespace Cotorra.WebAPI.Controllers
{

    /// <summary>
    ///  Cotorria's Calculation API
    /// </summary>
    /// <seealso cref="Cotorra.WebAPI.Controllers.BaseCotorraController" />
    [Route("api/payroll/[controller]")]
    [ApiController]
    public class CalculationController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly ICalculationService _service;

        public CalculationController(IMapper mapper, ICalculationService service)
        {
            _mapper = mapper;
            _service = service;
        }


        /// <summary>
        /// Calculates an overdraft
        /// </summary>
        /// <returns>Specified Overdraft with calculous</returns>
        [HttpPost("CalculateOverdraft")]
        [CotorriaSecurity]
        public async Task<ActionResult<OverdraftDI>> CalculateOverdraft(CalculateOverdraftDIParams parameters)
        {
            OverdraftDI res = await _service.CalculateOverdraft(parameters, _mapper);
            res.OverdraftDetailDIs = res.OverdraftDetailDIs.OrderBy(p => (int)p.ConceptPaymentDI.ConceptType).ThenBy(p => p.ConceptPaymentDI.Code).ToList();
            return Ok(res);
        }

        /// <summary>
        /// Calculates one formula
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpPost("CalculateFormula")]
        [CotorriaSecurity]
        public async Task<ActionResult<CalculateResult>> CalculateFormula(CalculateOverdraftDIParams parameters)
        {
            CalculateResult res = await _service.CalculateFormula(parameters);
            return Ok(res);
        }



    }


}
