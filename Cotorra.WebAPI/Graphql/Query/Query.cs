using AutoMapper;
using GreenDonut;
using HotChocolate;
using MoreLinq;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
using Cotorra.Schema.DTO.Catalogs;
using Cotorra.WebAPI.Graphql;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{


    public class Query
    {
        [GraphQLNonNullType]
        public async Task<IEnumerable<EmployeeDTO>> GetEmployees(string instanceID, string companyID)
        {
            List<EmployeeDTO> result = new List<EmployeeDTO>();
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Employee, EmployeeDTO>();
            });

            var _mapper = config.CreateMapper();

            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = await middlewareManager.GetAllAsync(Guid.Parse(companyID), Guid.Parse(instanceID), new string[] { "JobPosition", "Department" });

            employees.ForEach(employee =>
            {
                var employeeDTO = _mapper.Map<Employee, EmployeeDTO>(employee);
                employeeDTO.DepartmentDTO.Name = employee.Department.Name;
                employeeDTO.JobPositionDTO.Name = employee.JobPosition.Name;

                result.Add(employeeDTO);
            });

            return result;
        }


        public async Task<EmployeeDTO> GetEmployee(IDDataloaderParam id, [DataLoader] EmployeeDataLoader loader)
        {
            return await loader.LoadAsync(id);
        }


        public async Task<DepartmentDTO> GetDepartment(IDDataloaderParam id, [DataLoader] DepartmentDataLoader loader)
        {
            return await loader.LoadAsync(id);
        }

        public async Task<List<DepartmentDTO>> GetDepartments(AllDataloaderParam id, [DataLoader] DepartmentsDataLoader loader)
        {
            return await loader.LoadAsync(id);
        }

        [GraphQLNonNullType]
        public async Task<EmployeeDTO> GetEmployeeIdentity(IdentityIDDataloaderParam id, [DataLoader] EmployeeIdentityDataLoader loader)
        {
            var res = await loader.LoadAsync(id);
            return res; 
        }


        [GraphQLNonNullType]
        public async Task<IEnumerable<OverdraftDTO>> GetOverdraftEmployeeId(IDEmployeeDataloaderParam id, [DataLoader] OverdraftDataLoader loader)
        {
            return await loader.LoadAsync(id);          
        }


        [GraphQLNonNullType]
        public async Task<EmployeeBenefitsDTO> GetEmployeeBenefits(string instanceID, string companyID, string employeeID)
        {

            var middlewareManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employee = (await middlewareManager.GetByIdsAsync(new List<Guid>() { Guid.Parse(employeeID) }, Guid.Parse(companyID),
                new string[] { })).FirstOrDefault();


            var entryDate = employee.EntryDate;
            var employeeTrustLevel = employee.EmployeeTrustLevel == EmployeeTrustLevel.Unionized ? "Sindicalizado" : "Confianza";
            var now = DateTime.Now;

            var currentYear = entryDate.Year - now.Year + 1;

            var benefitManager = new MiddlewareManager<BenefitType>(new BaseRecordManager<BenefitType>(), new BenefitTypeValidator());
            var benefit = (await benefitManager.FindByExpressionAsync(x => x.Name == employeeTrustLevel && x.Antiquity == currentYear, Guid.Parse(companyID),
                new string[] { })).FirstOrDefault();

            var res = new EmployeeBenefitsDTO()
            {
                VacationalBonusPerSeniority = Math.Round(benefit.HolidayPremiumPortion, 2).ToString(),
                VacationsPerSeniority = Math.Round(benefit.Holidays, 2).ToString(),
                YearlyBonusPerSeniority = Math.Round(benefit.DaysOfChristmasBonus,2).ToString(),
                PendingVacationDays = "4",
            };




            return res;
        }
    }

}
