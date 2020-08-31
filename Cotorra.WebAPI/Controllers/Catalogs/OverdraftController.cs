using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;
using Cotorra.Core.Managers;
using Cotorra.Core.Utils;

namespace Cotorra.WebAPI.Controllers.Catalogs
{
    [Route("api/[controller]")]
    [ApiController]
    public class OverdraftController : BaseCotorraController
    {
        private readonly IMapper _mapper;

        public OverdraftController()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<Overdraft, OverdraftDTO>();
            });

            _mapper = config.CreateMapper();            
        }

        [HttpGet("GetByEmployeeId/{companyId}/{instanceId}/{employeeId}")]
        //[Security(ServiceID = PermissionContants.SuperMamalonPermission.ServiceID,
        //    UseAuthorization = false,
        //    UseSession = false,
        //    Permissions = new[] { PermissionContants.SuperMamalonPermission.PermissionID },
        //    ResourceID = PermissionContants.SuperMamalonPermission.CotoRRA_Cloud_ID_String,
        //    UserInstanceAsOwner = false)]
        public async Task<ActionResult<string>> GetByEmployeeId(Guid companyId, Guid instanceId, Guid employeeId)
        {
            List<OverdraftDTO> overdraftDTOs = new List<OverdraftDTO>();
            JsonResult result = new JsonResult(overdraftDTOs);
            var _blobStorageUtil = new BlobStorageUtil(instanceId);
            await _blobStorageUtil.InitializeAsync();
            try
            {
                var middlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(),
                    new OverdraftValidator());

                //get historic overdraft last 3 months and stamped only
                var overdrafts = await middlewareManager.FindByExpressionAsync(p =>
                    p.company == companyId &&
                    p.InstanceID == instanceId &&
                    p.HistoricEmployee.EmployeeID == employeeId &&
                    p.Active && p.OverdraftStatus == OverdraftStatus.Stamped,
                    companyId, new string[] {
                        "HistoricEmployee",
                        "PeriodDetail",
                        "PeriodDetail.Period",
                        "OverdraftDetails", 
                        "OverdraftDetails.ConceptPayment" });

                if (overdrafts.Any())
                {
                    var overdraftManager = new OverdraftManager();
                   
                    await overdrafts.ForEachAsync(async historicOverdraft =>
                    {
                        var totals = overdraftManager.GetTotals(historicOverdraft, new Core.Utils.RoundUtil("MXN"));

                        //Fill DTO
                        var overdraftDTO = _mapper.Map<Overdraft, OverdraftDTO>(historicOverdraft);
                        overdraftDTO.Total = totals.Total;
                        overdraftDTO.TotalPerceptions = totals.TotalSalaryPayments;
                        overdraftDTO.TotalDeductions = totals.TotalDeductionPayments;
                        overdraftDTO.InitialDate = historicOverdraft.PeriodDetail.InitialDate;
                        overdraftDTO.FinalDate = historicOverdraft.PeriodDetail.FinalDate;
                        //Period Traduction
                        overdraftDTO.PeriodTypeName = PaymentPeriodicityTraduction.GetTraduction(historicOverdraft.PeriodDetail.Period.PaymentPeriodicity);

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

                        //Perceptions and Deductions
                        historicOverdraft.OverdraftDetails.OrderBy(p=>p.ConceptPayment.Code).ToList().ForEach(detail =>
                        {
                            if (detail.ConceptPayment.ConceptType == ConceptType.SalaryPayment)
                            {
                                overdraftDTO.Perceptions.Add(new PerceptionDTO() { Description = detail.ConceptPayment.Name, Amount = detail.Amount });
                            }
                            else if (detail.ConceptPayment.ConceptType == ConceptType.DeductionPayment)
                            {
                                overdraftDTO.Deductions.Add(new DeductionDTO() { Description = detail.ConceptPayment.Name, Amount = detail.Amount });
                            }
                        });

                        overdraftDTOs.Add(overdraftDTO);
                    }, Environment.ProcessorCount);

                    result = new JsonResult(overdraftDTOs);
                }
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    var exception = ex;
                    while (exception.Message.Contains("Exception has been thrown by the target of an invocation"))
                    {
                        exception = exception.InnerException;
                    }

                    throw exception;
                }
            }
            return result;
        }

    }
}
