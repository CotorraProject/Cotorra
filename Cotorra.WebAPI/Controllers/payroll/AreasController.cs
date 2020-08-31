using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
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
    public class AreasController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly IAreasService _service;
        private readonly ICatalogServiceSearch<Area, AreaDTO> _searchService;


        public AreasController(IMapper mapper, ICatalogServiceSearch<Area, AreaDTO> service, IAreasService areasService)
        {
            _mapper = mapper;
            _searchService = service;
            _service = areasService;
        }


        /// <summary>
        /// Gets all the Areas
        /// </summary>
        /// <returns>Specified Area</returns>
        [HttpGet]
        [CotorriaSecurity]
        public async Task<ActionResult<IEnumerable<AreaDTO>>> Get()
        {
            IEnumerable<AreaDTO> res = await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { }, new AreaValidator(), _mapper);
            return Ok(res);
        }


        /// <summary>
        /// Gets the specified Area
        /// </summary>
        /// <param name="AreaID">Area's Identifier </param>
        /// <returns>All the departments</returns>
        [HttpGet("{AreaID}")]
        [CotorriaSecurity]
        public async Task<ActionResult<AreaDTO>> GetByID(Guid AreaID)
        {
            var res = (await _searchService.Get(x => x.InstanceID == this.InstanceID
                     && x.company == this.IdentityWorkID && x.ID == AreaID
                     && x.Active, this.IdentityWorkID, new string[] { }, new AreaValidator(), _mapper)).FirstOrDefault();

            if (res != null)
            {
                return Ok(res);
            }

            return NotFound();
        }

        /// <summary>
        /// Creates the specified Area
        /// </summary>
        /// <param name="area">Area to Create </param>
        /// <returns>All the departments</returns>
        [HttpPost]
        [CotorriaSecurity]
        public async Task<ActionResult<AreaDTO>> Post(AreaDTO area)
        {
            var payload = await _service.Post(this.InstanceID, this.IdentityID, this.IdentityWorkID, new AreaValidator(), _mapper, area);
            if (payload != null)
            {
                return Ok(payload);
            }

            return Ok();
        }


        /// <summary>
        /// Updates the specified Area
        /// </summary>
        /// <param name="area">Area to Create </param>
        /// <returns>All the departments</returns>
        [HttpPut]
        [CotorriaSecurity]
        public async Task<ActionResult<AreaDTO>> Put(AreaDTO area)
        {
            var departmentPayload = await _service.Put(this.InstanceID, this.IdentityID, this.IdentityWorkID, new AreaValidator(), _mapper, area);
            if (departmentPayload != null)
            {
                return Ok(departmentPayload);
            }

            return Ok();
        }

        /// <summary>
        /// Deletes the specified Area
        /// </summary>
        /// <param name="AreaID">Area's ID To Delete </param>
        /// <returns>All the departments</returns>
        [HttpDelete]
        [CotorriaSecurity]
        public async Task<ActionResult<AreaDTO>> Delete(Guid AreaID)
        {
            await _service.Delete(this.InstanceID, this.IdentityID, this.IdentityWorkID, new AreaValidator(), _mapper, AreaID);
            return Ok();
        }



    }


}
