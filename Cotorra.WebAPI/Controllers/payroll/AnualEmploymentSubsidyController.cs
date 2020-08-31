using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.WebAPI.ActionFilters;
using Cotorra.Schema.DTO;

namespace Cotorra.WebAPI.Controllers.Catalogs
{

    /// <summary>
    ///  Cotorria's Departments API
    /// </summary>
    /// <seealso cref="Cotorra.WebAPI.Controllers.BaseCotorraController" />
    [Route("api/payroll/[controller]")]
    [ApiController]
    public class AnualEmploymentSubsidyController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly ICatalogServiceSearch<AnualEmploymentSubsidy, AnualEmploymentSubsidyDTO> _searchService;


        public AnualEmploymentSubsidyController(IMapper mapper, ICatalogServiceSearch<AnualEmploymentSubsidy, AnualEmploymentSubsidyDTO> service)
        {
            _mapper = mapper;
            _searchService = service; 
        }


        /// <summary>
        /// Gets all the departments
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet]
        [CotorriaSecurity]
        public async Task<ActionResult<IEnumerable<AnualEmploymentSubsidyDTO>>> Get()
        {
            IEnumerable<AnualEmploymentSubsidyDTO> res = await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { }, new AnualEmploymentSubsidyValidator(), _mapper);
            return Ok(res);
        }


    }


}
