using AutoMapper;
using GreenDonut;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cotorra.WebAPI.Graphql
{


    public class EmployeeDataLoader : DataLoaderBase<IDDataloaderParam, EmployeeDTO>
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeDataLoader()
          : base(new DataLoaderOptions<IDDataloaderParam>())
        {
            _repository = new EmployeeRepository();
        }

        protected override async Task<IReadOnlyList<Result<EmployeeDTO>>> FetchAsync(IReadOnlyList<IDDataloaderParam> keys, CancellationToken cancellationToken)
        {
            return await _repository.GetEmplooyees(keys);
        }
    }

    public class EmployeeIdentityDataLoader : DataLoaderBase<IdentityIDDataloaderParam, EmployeeDTO>
    {
        private readonly IEmployeeRepository _repository;

        public EmployeeIdentityDataLoader()
          : base(new DataLoaderOptions<IdentityIDDataloaderParam>())
        {
            _repository = new EmployeeRepository();
        }

        protected override async Task<IReadOnlyList<Result<EmployeeDTO>>> FetchAsync(IReadOnlyList<IdentityIDDataloaderParam> keys, CancellationToken cancellationToken)
        {
            return await _repository.GetEmplooyees(keys);
        }
    }


    public interface IEmployeeRepository
    {
        Task<IReadOnlyList<Result<EmployeeDTO>>> GetEmplooyees(IReadOnlyList<IDDataloaderParam> data);
        Task<IReadOnlyList<Result<EmployeeDTO>>> GetEmplooyees(IReadOnlyList<IdentityIDDataloaderParam> data);

    }

    public class EmployeeRepository : IEmployeeRepository
    {
        public async Task<IReadOnlyList<Result<EmployeeDTO>>> GetEmplooyees(IReadOnlyList<IDDataloaderParam> data)
        {

            var instanceID = data.FirstOrDefault().instanceID;
            var companyID = data.FirstOrDefault().companyID;
            var ids = data.Select(x => Guid.Parse(x.id)).ToList();
            List<Result<EmployeeDTO>> result = new List<Result<EmployeeDTO>>();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Employee, EmployeeDTO>();
            });

            var _mapper = config.CreateMapper();

            EmployeeDTO employeeDTO = new EmployeeDTO();

            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == Guid.Parse(instanceID) &&
                                p.company == Guid.Parse(companyID) && ids.Contains(p.ID) && p.Active, Guid.Empty,
                new string[] { "JobPosition", "Department" });

            employees.ForEach(employee =>
            {
                var employeeDTO = _mapper.Map<Employee, EmployeeDTO>(employee);
                employeeDTO.DepartmentDTO.Name = employee.Department.Name;
                employeeDTO.JobPositionDTO.Name = employee.JobPosition.Name;

                result.Add(employeeDTO);
            });


            return result;

        }

        public async Task<IReadOnlyList<Result<EmployeeDTO>>> GetEmplooyees(IReadOnlyList<IdentityIDDataloaderParam> data)
        {
            var instanceID = data.FirstOrDefault().instanceID;
            var companyID = data.FirstOrDefault().companyID;
            var identityIds = data.Select(x => (Guid?)Guid.Parse(x.identityID)).ToList();
            List<Result<EmployeeDTO>> result = new List<Result<EmployeeDTO>>();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Employee, EmployeeDTO>();
            });

            var _mapper = config.CreateMapper();

            EmployeeDTO employeeDTO = new EmployeeDTO();

            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = await middlewareManager.FindByExpressionAsync(p => identityIds.Contains(p.IdentityUserID)
               && p.Active,
                Guid.Empty,
                new string[] { "JobPosition", "Department" });


            employees.ForEach(employee =>
            {
                var employeeDTO = _mapper.Map<Employee, EmployeeDTO>(employee);
                employeeDTO.DepartmentDTO.Name = employee.Department.Name;

                employeeDTO.JobPositionDTO.Name = employee.JobPosition.Name;


                result.Add(employeeDTO);
            });


            return result;
        }
    }



}
