using AutoMapper;
using GreenDonut;
using Cotorra.Core;
using Cotorra.Core.Managers;
using Cotorra.Core.Utils;
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


    public class OverdraftDataLoader : DataLoaderBase<IDEmployeeDataloaderParam, List<OverdraftDTO>>
    {
        private readonly IOverdraftRepository _repository;

        public OverdraftDataLoader( )
          : base(new DataLoaderOptions<IDEmployeeDataloaderParam>())
        {
            _repository = new OverdraftRepository();
        }

        protected override async Task<IReadOnlyList<Result<List<OverdraftDTO>>>> FetchAsync(IReadOnlyList<IDEmployeeDataloaderParam> keys, CancellationToken cancellationToken)
        {
            return await _repository.GetOverByEmployeeID(keys);
        }
    }

    

    public interface IOverdraftRepository
    {
        Task<IReadOnlyList<Result<List<OverdraftDTO>>>> GetOverByEmployeeID(IReadOnlyList<IDEmployeeDataloaderParam> data);
       
    }

    public class OverdraftRepository : IOverdraftRepository
    {
        public async Task<IReadOnlyList<Result<List<OverdraftDTO>>>> GetOverByEmployeeID(IReadOnlyList<IDEmployeeDataloaderParam> data)
        {
            var instanceID = data.FirstOrDefault().instanceID;
            var companyID = data.FirstOrDefault().companyID;
            var employeeIDs = data.Select(x => (Guid?)Guid.Parse(x.employeeID )).ToList();

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Overdraft, OverdraftDTO>();
            });
            var _blobStorageUtil = new BlobStorageUtil(Guid.Parse(instanceID));
            await _blobStorageUtil.InitializeAsync();

            var _mapper = config.CreateMapper(); 

            List<Result<List<OverdraftDTO>>> result = new List<Result<List<OverdraftDTO>>>();


            var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                               new OverdraftValidator());

            var historicOverdrafts = await middlewareManager.FindByExpressionAsync(p =>
                     p.company == Guid.Parse(companyID) &&
                     p.InstanceID == Guid.Parse(instanceID) &&
                     employeeIDs.Contains( p.HistoricEmployee.EmployeeID ) &&
                     p.PeriodDetail.FinalDate >= DateTime.UtcNow.AddMonths(-3) &&
                     p.Active && p.OverdraftStatus == OverdraftStatus.Stamped,
                     Guid.Parse(companyID), new string[] { "HistoricEmployee", "PeriodDetail",
                         "OverdraftDetails", "OverdraftDetails.ConceptPayment" });

            await employeeIDs.ForEachAsync(async empID =>
            {
                var overs = historicOverdrafts.Where(x => x.EmployeeID == empID);
                List<OverdraftDTO> overdraftDTOs = new List<OverdraftDTO>();

                if (overs.Any())
                {
                    var overdraftManager = new OverdraftManager();

                    await overs.ForEachAsync(async historicOverdraft =>
                    {
                        var totals = overdraftManager.GetTotals(historicOverdraft, new Core.Utils.RoundUtil("MXN"));

                        //Fill DTO
                        var overdraftDTO = _mapper.Map<Overdraft, OverdraftDTO>(historicOverdraft);
                        overdraftDTO.Total = totals.Total;
                        overdraftDTO.TotalPerceptions = totals.TotalSalaryPayments;
                        overdraftDTO.TotalDeductions = totals.TotalDeductionPayments;
                        overdraftDTO.InitialDate = historicOverdraft.PeriodDetail.InitialDate;
                        overdraftDTO.FinalDate = historicOverdraft.PeriodDetail.FinalDate;
                        try
                        {
                            if (overdraftDTO.UUID != Guid.Empty)
                            {
                                overdraftDTO.XML = await _blobStorageUtil.DownloadDocumentAsync($"{overdraftDTO.UUID}.xml");
                            }
                        }
                        catch
                        {
                            //No se pudo obtener el xml asociado. 
                            overdraftDTO.XML = String.Empty;
                        }

                        overdraftDTOs.Add(overdraftDTO);
                    }, Environment.ProcessorCount);
                    result.Add(overdraftDTOs);
                }
            }, Environment.ProcessorCount); 
            return result;

        }
         
    }

  

}
