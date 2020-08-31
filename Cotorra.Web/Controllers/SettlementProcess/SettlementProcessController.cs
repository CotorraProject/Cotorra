using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class SettlementProcessController : Controller
    {
        private readonly Client<Overdraft> client;
        private readonly Client<Employee> employeeClient;
        private readonly StatusClient<Employee> statusClient;
        private readonly ISettlementProcessClient settlementProcessClient;


        public SettlementProcessController()
        {
            SessionModel.Initialize();
            client = new Client<Overdraft>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
            settlementProcessClient = new SettlementProcessClient(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
            employeeClient = new Client<Employee>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
            statusClient = new StatusClient<Employee>(SessionModel.AuthorizationHeader, ClientConfiguration.GetAdapterFromConfig());
        }


        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetEmployees(Guid periodTypeID)
        {
            var employees = (await employeeClient.FindAsync(x => x.InstanceID == SessionModel.InstanceID &&
                x.PeriodTypeID == periodTypeID, SessionModel.CompanyID)).AsParallel().OrderBy(x => x.Code.Length).ThenBy(p => p.Code);
            return Json(employees);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get(Guid overdraftID)
        {
            var result = await client.FindAsync(x => x.InstanceID == SessionModel.InstanceID && x.ID == overdraftID,
                SessionModel.CompanyID, new string[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment" });
                       
            var settlement = (from o in result
                              select new
                              {
                                  o.ID,
                                  Concepts = (from od in o.OverdraftDetails
                                              orderby od.ConceptPayment.Code
                                              select new
                                              {
                                                  od.ID,
                                                  od.ConceptPaymentID,
                                                  ConceptPaymentName = od.ConceptPayment.Name,
                                                  ConceptPaymentCode = od.ConceptPayment.Code,
                                                  ConceptPaymentType = od.ConceptPayment.ConceptType,
                                                  ConceptPaymentKind = od.ConceptPayment.Kind,
                                                  ConceptPaymentPrint = od.ConceptPayment.Print,
                                                  od.Value,
                                                  od.Amount,
                                                  od.IsGeneratedByPermanentMovement,
                                                  od.IsValueCapturedByUser,
                                                  od.IsTotalAmountCapturedByUser,
                                              })
                              });

            return Json("");
        }


        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Calculate(CalculateSettlementProcessParams data)
        {
            data.IdentityWorkID = SessionModel.CompanyID;
            data.InstanceID = SessionModel.InstanceID;
            data.user = SessionModel.IdentityID;
            var res = await settlementProcessClient.Calculate(data);
            return Json(res);
        }


        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> GenerateSettlementLetter(Guid overdraftID)
        {

            var res = await settlementProcessClient.GenerateSettlementLetter(new GenerateSettlementLetterParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                OverdraftIDs = new List<Guid>() { overdraftID },
                Token = SessionModel.AuthorizationHeader
            });
            return Json(res);
        }


        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetActivePeriodTypes()
        {
            var clientPT = new Client<PeriodType>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());
            var findResult = await clientPT.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID,
                SessionModel.CompanyID, new String[] { "Periods", "Periods.PeriodDetails" });

            var periodTypes = from pt in findResult
                              orderby pt.ExtraordinaryPeriod, pt.PeriodTotalDays
                              where pt.Periods.Count() > 0 && !pt.ExtraordinaryPeriod
                              select new
                              {
                                  ID = pt.ID,
                                  Name = pt.Name,
                                  CurrentYear = pt.Periods.OrderByDescending(x => x.FiscalYear).First().FiscalYear,

                                  Years = (from p in pt.Periods orderby p.FiscalYear descending select new { ID = p.ID, Year = p.FiscalYear }),

                                  PeriodID = pt.Periods.OrderByDescending(x => x.FiscalYear).First().ID,

                                  PeriodDetailID = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating).OrderBy(x => x.Number).First().ID,

                                  Number = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating).OrderBy(x => x.Number).First().Number,

                                  InitialDate = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating).OrderBy(x => x.Number).First().InitialDate,

                                  FinalDate = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating).OrderBy(x => x.Number).First().FinalDate,

                                  IsExtraordinary = false
                              };


            return Json(periodTypes);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetFiscalYears(Guid periodTypeID)
        {
            var client = new Client<Period>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());
            var findResult = await client.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID && x.PeriodTypeID == periodTypeID,
                SessionModel.CompanyID, new String[] { });

            var periodTypes = from p in findResult
                              orderby p.FiscalYear descending
                              select new
                              {
                                  ID = p.ID,
                                  Name = p.FiscalYear
                              };


            return Json(periodTypes);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetPeriodDetails(Guid periodID)
        {
            var client = new Client<PeriodDetail>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());
            var findResult = await client.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID && x.PeriodID == periodID,
                SessionModel.CompanyID, new String[] { });

            var periodDetails = from p in findResult
                                orderby p.Number
                                select new
                                {
                                    p.ID,
                                    p.Number,
                                    p.PeriodStatus,
                                    p.InitialDate,
                                    p.FinalDate
                                };

            return Json(periodDetails);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetWorkPeriod(Guid periodTypeID, Guid? periodDetailID = null)
        {
            //Set clients
            var conceptsClient = new Client<ConceptPayment>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());
            var overdraftClient = new Client<Overdraft>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());
            var incidentsClient = new Client<Incident>(SessionModel.AuthorizationHeader, clientadapter: ClientConfiguration.GetAdapterFromConfig());

            //Get grid columns (concepts marked as global automatic)
            var conceptsResult = conceptsClient.FindAsync(x =>
                x.ConceptType != ConceptType.LiabilityPayment &&
                x.Active && x.InstanceID == SessionModel.InstanceID && x.GlobalAutomatic && !x.Kind,
                SessionModel.CompanyID, new String[] { });

            //Get the overdrafts
            var overdraftsResult = overdraftClient.FindAsync(x =>
                x.PeriodDetailID == periodDetailID &&
                x.Active && x.InstanceID == SessionModel.InstanceID,
                SessionModel.CompanyID, new String[] { "Employee", "Employee.JobPosition", "OverdraftDetails" });

            //Get incidents
            var indicentsResult = incidentsClient.FindAsync(x =>
               x.PeriodDetailID == periodDetailID &&
               x.Active && x.InstanceID == SessionModel.InstanceID,
                SessionModel.CompanyID, new String[] { "IncidentType" });


            await Task.WhenAll(conceptsResult, overdraftsResult, indicentsResult);
                      
            //Create the result
            var result = new
            {
                Employees = (from o in overdraftsResult.Result
                             orderby o.Employee.Code
                             select new
                             {
                                 o.Employee.ID,
                                 o.Employee.Code,
                                 o.Employee.DailySalary,
                                 JobPositionName = o.Employee.JobPosition.Name,
                                 FullName = o.Employee.FirstLastName +
                                        (!String.IsNullOrEmpty(o.Employee.SecondLastName) ? (" " + o.Employee.SecondLastName) : "") + " " +
                                        o.Employee.Name,
                             }).ToList(),

                DPOColumns = (from concept in conceptsResult.Result
                              orderby concept.ConceptType, concept.Code
                              select new
                              {
                                  ConceptPaymentID = concept.ID,
                                  field = "F_" + concept.ID.ToString().Replace("-", ""),
                                  title = concept.Name,
                              }).ToList(),

                DPOData = (from o in overdraftsResult.Result
                           orderby o.Employee.Code
                           select new
                           {

                               o.ID,
                               o.EmployeeID,
                               Details = (from d in o.OverdraftDetails
                                          select new
                                          {
                                              d.ConceptPaymentID,
                                              d.Amount,
                                              //Amount = random.Next(1, 9999),
                                              d.IsValueCapturedByUser,
                                              d.IsGeneratedByPermanentMovement,
                                          }).ToList()
                           }).ToList(),

                DHData = (from i in indicentsResult.Result
                          select new
                          {
                              //i.ID,
                              i.EmployeeID,
                              i.Date,
                              i.Value,
                              i.IncidentType.Code,

                          }).ToList()
            };

            return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetSettlementDefaultSettlementDataItems()
        {

            var result = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var at = from r in result
                     orderby r.Name
                     select new
                     {
                         r.ID,
                         r.Name,
                     };

            return Json(at);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> ApplySettlement(ApplySettlementProcessParams applySettlementParameters)
        {
            applySettlementParameters.InstanceID = SessionModel.InstanceID;
            applySettlementParameters.user = SessionModel.IdentityID;
            applySettlementParameters.IdentityWorkID = SessionModel.CompanyID;
            applySettlementParameters.LicenseServiceID = SessionModel.LicenseServiceID;
            applySettlementParameters.LicenseID = SessionModel.LicenseID;
            applySettlementParameters.AuthTkn = SessionModel.AuthorizationHeader;

            await settlementProcessClient.ApplySettlement(applySettlementParameters);
            return Json("OK");
        }



    }

}
