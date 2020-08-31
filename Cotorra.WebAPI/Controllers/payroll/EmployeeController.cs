using AutoMapper;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.WebAPI.ActionFilters;
using Cotorra.Schema.DTO.Catalogs;
using System;
using System.Linq;
using Cotorra.Core.Utils;

namespace Cotorra.WebAPI.Controllers.Catalogs
{

    /// <summary>
    ///  Cotorria's Employees API
    /// </summary>
    /// <seealso cref="Cotorra.WebAPI.Controllers.BaseCotorraController" />
    [Route("api/payroll/[controller]")]
    [ApiController]
    public class EmployeeController : BaseCotorraController
    {
        private readonly IMapper _mapper;
        private readonly ICatalogServiceSearch<Employee, EmployeeDTO> _searchService;


        public EmployeeController(IMapper mapper, ICatalogServiceSearch<Employee, EmployeeDTO> service)
        {
            _mapper = mapper;
            _searchService = service;
        }


        /// <summary>
        /// Gets all the Employees
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet]
        [CotorriaSecurity]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> Get()
        {
            IEnumerable<EmployeeDTO> res = await _searchService.Get(x => x.InstanceID == this.InstanceID && x.company == this.IdentityWorkID &&
               x.Active, this.IdentityWorkID, new string[] { "JobPosition", "Department" }, new EmployeeValidator(), _mapper);

            res.AsParallel().ForAll(emp => emp.Antiquity = new CalculateDateDifference(DateTime.Now, emp.EntryDate).ToStringColaboratorSpanish());
            return Ok(res);
        }


        /// <summary>
        /// Gets all the Employees
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet("GetByIdentityId/{identityUserID}")]
        //[CotorriaSecurity]

        public async Task<ActionResult<string>> GetByIdentityId(Guid identityUserID)
        {

            EmployeeDTO emp = (await _searchService.Get(x => x.IdentityUserID == identityUserID 
                //&& x.InstanceID == this.InstanceID
                //&& x.company == this.IdentityWorkID
                && x.Active, this.IdentityWorkID, new string[] { "JobPosition", "Department" }, new EmployeeValidator(), _mapper)).FirstOrDefault();
            if (emp != null)
            {
                emp.Antiquity = new CalculateDateDifference(DateTime.Now, emp.EntryDate).ToStringColaboratorSpanish();
                return Ok(emp);
            }
            return NotFound();
        }


        /// <summary>
        /// Gets all the Employees
        /// </summary>
        /// <returns>Specified Department</returns>
        [HttpGet("GetById/{employeeId}")]
        [CotorriaSecurity]

        public async Task<ActionResult<string>> GetById(Guid employeeId)
        {
            EmployeeDTO emp = (await _searchService.Get(x => x.ID == employeeId
                 && x.InstanceID == this.InstanceID
                 && x.company == this.IdentityWorkID
                 && x.Active, this.IdentityWorkID, new string[] { "JobPosition", "Department" }, new EmployeeValidator(), _mapper)).FirstOrDefault();
            if (emp != null)
            {
                emp.Antiquity = new CalculateDateDifference(DateTime.Now, emp.EntryDate).ToStringColaboratorSpanish();
                return Ok(emp);
            }
            return NotFound();
        }


    }


}
