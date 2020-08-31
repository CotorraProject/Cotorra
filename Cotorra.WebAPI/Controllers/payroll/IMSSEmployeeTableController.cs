using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.WebAPI.ActionFilters; 

namespace Cotorra.WebAPI.Controllers.Catalogs
{

    /// <summary>
    ///  Cotorria's IMSS Employer API
    /// </summary>
    /// <seealso cref="Cotorra.WebAPI.Controllers.BaseCotorraController" />
    [Route("api/payroll/[controller]")]
    [ApiController]
    public class IMSSEmployeeTableController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly ICatalogServiceSearch<IMSSEmployeeTable, IMSSEmployeeTableDTO> _searchService;


        public IMSSEmployeeTableController(IMapper mapper, ICatalogServiceSearch<IMSSEmployeeTable, IMSSEmployeeTableDTO> service)
        {
            _mapper = mapper;
            _searchService = service; 
        }


        /// <summary>
        /// Gets all the IMSS employer fees
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet]
        [CotorriaSecurity]
        public async Task<ActionResult<IEnumerable<IMSSEmployeeTableDTO>>> Get()
        { 
            return Ok(await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { }, new IMSSEmployeeTableValidator(), _mapper));
        }


    }


}
