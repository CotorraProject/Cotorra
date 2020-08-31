using AutoMapper;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    public class DepartmentService : IDepartmentService
    {
        public async Task<DepartmentDTO> Post(Guid instanceID, Guid userID, Guid identityWorkID, DepartmentValidator validator, IMapper mapper,
            DepartmentDTO departmentDTO)
        {
            Department dep = new Department();
            mapper.Map(departmentDTO, dep);
            dep.InstanceID = instanceID;
            dep.CompanyID = identityWorkID;

            var mgr = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), validator);
            await mgr.CreateAsync(new List<Department>() { dep }, identityWorkID);
            return departmentDTO;
        }

        public async Task<DepartmentDTO> Put(Guid instanceID, Guid userID, Guid identityWorkID, DepartmentValidator validator, IMapper mapper, DepartmentDTO DepartmentDTO)
        {
            var mgr = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), validator);
            var found = (await mgr.FindByExpressionAsync(x => x.ID == DepartmentDTO.ID, identityWorkID, new string[] { "Area" })).FirstOrDefault();


            if (found != null)
            {
                found.Name = DepartmentDTO.Name;
                await mgr.UpdateAsync(new List<Department>() { found }, identityWorkID);
            }
            else
            {
                return await Post(instanceID, userID, identityWorkID, validator, mapper, DepartmentDTO);
            }

            return DepartmentDTO;
        }



    }

}
