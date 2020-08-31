using AutoMapper;
using GreenDonut;
using MoreLinq;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Cotorra.WebAPI.Graphql
{


    public class DepartmentDataLoader : DataLoaderBase<IDDataloaderParam, DepartmentDTO>
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentDataLoader()
          : base(new DataLoaderOptions<IDDataloaderParam>())
        {
            _repository = new DepartmentRepository();
        }

        protected override async Task<IReadOnlyList<Result<DepartmentDTO>>> FetchAsync(IReadOnlyList<IDDataloaderParam> keys, CancellationToken cancellationToken)
        {
            return await _repository.GetDepartments(keys);
        }
    }

    public class DepartmentsDataLoader : DataLoaderBase<AllDataloaderParam, List<DepartmentDTO>>
    {
        private readonly IDepartmentRepository _repository;

        public DepartmentsDataLoader()
          : base(new DataLoaderOptions<AllDataloaderParam>())
        {
            _repository = new DepartmentRepository();
        }

        protected override async Task<IReadOnlyList<Result<List<DepartmentDTO>>>> FetchAsync(IReadOnlyList<AllDataloaderParam> keys, CancellationToken cancellationToken)
        {
            return await _repository.GetDepartments(keys);
        }
         
    }


    public interface IDepartmentRepository
    {
        Task<IReadOnlyList<Result<DepartmentDTO>>> GetDepartments(IReadOnlyList<IDDataloaderParam> data);
        Task<IReadOnlyList<Result<List<DepartmentDTO>>>> GetDepartments(IReadOnlyList<AllDataloaderParam> data);

    }

    public class DepartmentRepository : IDepartmentRepository
    {
        public async Task<IReadOnlyList<Result<DepartmentDTO>>> GetDepartments(IReadOnlyList<IDDataloaderParam> data)
        {

            var instanceID = data.FirstOrDefault().instanceID;
            var companyID = data.FirstOrDefault().companyID;
            var ids = data.Select(x => Guid.Parse(x.id)).ToList();
            List<Result<DepartmentDTO>> result = new List<Result<DepartmentDTO>>();


            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Department, DepartmentDTO>();
            });

            var _mapper = config.CreateMapper();

            DepartmentDTO DepartmentDTO = new DepartmentDTO();

            var middlewareManager = new MiddlewareManager<Department>(new BaseRecordManager<Department>(), new DepartmentValidator());
            var departments = await middlewareManager.FindByExpressionAsync(p => p.InstanceID == Guid.Parse(instanceID) &&
                                p.company == Guid.Parse(companyID) && ids.Contains(p.ID) && p.Active, Guid.Empty);

            departments.ForEach(department =>
            {
                var DepartmentDTO = _mapper.Map<Department, DepartmentDTO>(department);
                DepartmentDTO.Name = department.Name;

                result.Add(DepartmentDTO);
            });


            return result;

        }

        public async Task<IReadOnlyList<Result<List<DepartmentDTO>>>> GetDepartments(IReadOnlyList<AllDataloaderParam> data)
        {
            var instanceIDs = data.Select(x => x.instanceID).AsParallel().Select(item => Guid.Parse(item)).ToList();
            var companyID = Guid.Parse(data.FirstOrDefault().companyID); 

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Department, DepartmentDTO>();
            });

            var _mapper = config.CreateMapper();


            List<Result<List<DepartmentDTO>>> result = new List<Result<List<DepartmentDTO>>>();


            var middlewareManager = new MiddlewareManager<Department>(new BaseRecordManager<Department>(),
                               new DepartmentValidator());

            var allInstancesDepartments = await middlewareManager.FindByExpressionAsync(p =>

                     instanceIDs.Contains(p.InstanceID)&& p.Active, companyID);

            var group = allInstancesDepartments.GroupBy(x => x.InstanceID);

            foreach (var item in group)
            {
                List<DepartmentDTO> depForKEy = new List<DepartmentDTO>();
                item.ForEach(department =>
                {

                    var DepartmentDTO = _mapper.Map<Department, DepartmentDTO>(department);
                    DepartmentDTO.Name = department.Name;
                     

                    depForKEy.Add(DepartmentDTO);
                });
                result.Add(depForKEy);
            }

 
            return result;

        }

    }



}
