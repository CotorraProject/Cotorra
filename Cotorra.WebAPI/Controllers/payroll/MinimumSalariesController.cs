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
    public class MinimumSalariesController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly ICatalogServiceSearch<MinimunSalary, MinimumSalaryDTO> _searchService;


        public MinimumSalariesController(IMapper mapper, ICatalogServiceSearch<MinimunSalary, MinimumSalaryDTO> service)
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
        public async Task<ActionResult<IEnumerable<MinimumSalaryDTO>>> Get()
        {
            IEnumerable<MinimumSalaryDTO> res = await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { }, new MinimunSalaryValidator(), _mapper);
            return Ok(res);
        }


    }


}
