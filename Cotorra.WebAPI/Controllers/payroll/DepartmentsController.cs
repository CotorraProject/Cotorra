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
    public class DepartmentsController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;
        private readonly ICatalogServiceSearch<Department, DepartmentDTO> _searchService;


        public DepartmentsController(IMapper mapper, ICatalogServiceSearch<Department, DepartmentDTO> service, IDepartmentService departmentService)
        {
            _mapper = mapper;
            _searchService = service;
            _departmentService = departmentService;
        }


        /// <summary>
        /// Gets all the departments
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet]
        [CotorriaSecurity]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> Get()
        {
            IEnumerable<DepartmentDTO> res = await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { "Area" }, new DepartmentValidator(), _mapper);
            return Ok(res);
        }


        /// <summary>
        /// Gets the specified department
        /// </summary>
        /// <param name="DepartmentID">Department's Identifier </param>
        /// <returns>All the departments</returns>
        [HttpGet("{DepartmentID}")]
        [CotorriaSecurity]
        public async Task<ActionResult<DepartmentDTO>> GetByID(Guid DepartmentID)
        { 
            DepartmentDTO department = (await _searchService.Get(x => x.InstanceID == this.InstanceID
                     && x.company == this.IdentityWorkID && x.ID == DepartmentID
                     && x.Active, this.IdentityWorkID, new string[] { "Area" }, new DepartmentValidator(), _mapper)).FirstOrDefault();

            if (department != null)
            {
                return Ok(department);

            } 

            return NotFound();
        }

        /// <summary>
        /// Gets the specified department
        /// </summary>
        /// <param name="department">Department to Create </param>
        /// <returns>All the departments</returns>
        [HttpPost]
        [CotorriaSecurity]
        public async Task<ActionResult<DepartmentDTO>> Post(DepartmentDTO department)
        {

           var departmentPayload = await _departmentService.Post(this.InstanceID, this.IdentityID, this.IdentityWorkID, new DepartmentValidator(), _mapper, department);


            if (departmentPayload != null)
            {
                return Ok(departmentPayload);
            }

            return Ok();
        }



    }


}
