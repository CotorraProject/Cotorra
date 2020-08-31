using Microsoft.AspNetCore.Mvc;
using Cotorra.Client;
using Cotorra.Schema;
using Cotorra.Web.Utils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class PrePayrollController : Controller
    {
        private readonly CalculationClient calculationClient;
        private readonly AuthorizeClient authorizeClient;
        private readonly Client<Period> clientPT;
        private readonly Client<PeriodDetail> clientPD;
        private readonly Client<Incident> incidentsClient;
        private readonly Client<Vacation> vacationsClient;
        private readonly Client<Inhability> inhabilitiesClient;
        private readonly Client<OverdraftDetail> clientOD;
        private readonly Client<Overdraft> clientO;
        private readonly Client<ConceptPayment> conceptsClient;
        private readonly StampingClient stampingClient;
        private readonly Client<catCFDI_CodigoPostal> clientCP;
        private readonly Client<PayrollCompanyConfiguration> clientPCC;
        private readonly OverdraftClient overdraftClient;

        public PrePayrollController()
        {
            SessionModel.Initialize();
            var configClientAdapter = ClientConfiguration.GetAdapterFromConfig();
            authorizeClient = new AuthorizeClient(SessionModel.AuthorizationHeader, configClientAdapter);
            clientPT = new Client<Period>(SessionModel.AuthorizationHeader, configClientAdapter);
            clientPD = new Client<PeriodDetail>(SessionModel.AuthorizationHeader, configClientAdapter);
            incidentsClient = new Client<Incident>(SessionModel.AuthorizationHeader, configClientAdapter);
            vacationsClient = new Client<Vacation>(SessionModel.AuthorizationHeader, configClientAdapter);
            inhabilitiesClient = new Client<Inhability>(SessionModel.AuthorizationHeader, configClientAdapter);
            clientOD = new Client<OverdraftDetail>(SessionModel.AuthorizationHeader, configClientAdapter);
            clientO = new Client<Overdraft>(SessionModel.AuthorizationHeader, configClientAdapter);
            conceptsClient = new Client<ConceptPayment>(SessionModel.AuthorizationHeader, configClientAdapter);
            calculationClient = new CalculationClient(SessionModel.AuthorizationHeader, configClientAdapter);
            stampingClient = new StampingClient(SessionModel.AuthorizationHeader, configClientAdapter);
            clientCP = new Client<catCFDI_CodigoPostal>(SessionModel.AuthorizationHeader, configClientAdapter);
            clientPCC = new Client<PayrollCompanyConfiguration>(SessionModel.AuthorizationHeader, configClientAdapter);
            overdraftClient = new OverdraftClient(SessionModel.AuthorizationHeader, configClientAdapter);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetActivePeriodTypes()
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var findResult = await clientPT.FindAsync(x =>
                x.InstanceID == SessionModel.InstanceID &&
                x.Active,
                SessionModel.CompanyID, new String[] { "PeriodType", "PeriodDetails" });

            var periodTypesPrevious = findResult.Select(p => p.PeriodType).ToList();

            var periodTypes = from pt in periodTypesPrevious
                              orderby pt.ExtraordinaryPeriod, pt.PeriodTotalDays
                              where pt.Periods.Count > 0 && !pt.ExtraordinaryPeriod
                              select new
                              {
                                  ID = pt.ID,
                                  Name = pt.Name,
                                  CurrentYear = pt.Periods.OrderByDescending(x => x.FiscalYear).First().FiscalYear,

                                  Years = (from p in pt.Periods orderby p.FiscalYear descending select new { ID = p.ID, Year = p.FiscalYear }),

                                  PeriodID = pt.Periods.OrderByDescending(x => x.FiscalYear).First().ID,

                                  PeriodDetailID = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized).OrderByDescending(x => x.Number).First().ID,

                                  Number = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized).OrderByDescending(x => x.Number).First().Number,

                                  InitialDate = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized).OrderByDescending(x => x.Number).First().InitialDate,

                                  FinalDate = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized).OrderByDescending(x => x.Number).First().FinalDate,

                                  PeriodStatus = pt.Periods.OrderByDescending(x => x.FiscalYear).First()
                                    .PeriodDetails.Where(x => x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized).OrderByDescending(x => x.Number).First().PeriodStatus,

                                  IsExtraordinary = false,


                              };

            stopwatch.Stop();
            Trace.WriteLine($"GetActivePeriods: {stopwatch.Elapsed}");
            return Json(periodTypes);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetFiscalYears(Guid periodTypeID)
        {
            var findResult = await clientPT.FindAsync(
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
            var findResult = await clientPD.FindAsync(
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
        public async Task<JsonResult> GetWorkPeriod(DateTime periodInitialDate, DateTime periodEndDate,
            Guid? periodDetailID = null, Guid? employeeID = null, Guid? overdraftID = null)
        {

            //Get overdrafts
            var overdraftsResult = clientO.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID && x.company == SessionModel.CompanyID && x.Active &&
                    x.PeriodDetailID == periodDetailID &&
                    x.EmployeeID == (employeeID == null ? x.EmployeeID : employeeID) &&
                    x.ID == (overdraftID == null ? x.ID : overdraftID),
                SessionModel.CompanyID,
                new String[] { "OverdraftDetails", "OverdraftDetails.ConceptPayment"/*, "HistoricEmployee" */});

            //Get incidents (días y horas)
            var indicentsResult = incidentsClient.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID && x.company == SessionModel.CompanyID && x.Active &&
                    x.PeriodDetailID == periodDetailID && (x.IncidentType.ItConsiders == ItConsiders.Absence || x.IncidentType.ItConsiders == ItConsiders.None) &&
                    x.EmployeeID == (employeeID == null ? x.EmployeeID : employeeID),
                SessionModel.CompanyID,
                new String[] { "IncidentType" });

            //Get vacations (vacaciones)
            var vacationsResult = vacationsClient.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID && x.company == SessionModel.CompanyID && x.Active &&
                    x.InitialDate >= periodInitialDate && x.FinalDate <= periodEndDate &&
                    x.EmployeeID == (employeeID == null ? x.EmployeeID : employeeID),
                SessionModel.CompanyID,
                new String[] { "VacationDaysOff" });

            //Get inhabilities (incapacidades)
            var inhabilitiesResult = inhabilitiesClient.FindAsync(x =>
                    x.InstanceID == SessionModel.InstanceID && x.company == SessionModel.CompanyID && x.Active &&
                    x.InitialDate >= periodInitialDate && x.InitialDate.AddDays(x.AuthorizedDays) <= periodEndDate &&
                    x.EmployeeID == (employeeID == null ? x.EmployeeID : employeeID),
                SessionModel.CompanyID,
                new String[] { "IncidentType" });

            //Get workperiod (new.... and fast!)
            var getWorkPeriodResult = overdraftClient.GetWorkPeriodAsync(
                SessionModel.CompanyID, SessionModel.InstanceID,
                periodDetailID, employeeID, overdraftID);

            //Execute requet
            await Task.WhenAll(getWorkPeriodResult, indicentsResult, vacationsResult, inhabilitiesResult);

            //TODO: GET HISTORIC EMPLOYEE WHEN ANY OVERDRAFT IS AUTHORIZED OR STAMPED (or maybe not)

            //Query them
            var result = new
            {
                DPOData = (from o in getWorkPeriodResult.Result.AsParallel()
                               //orderby o.Employee.Code
                           select new
                           {
                               //Overdraft
                               ID = o.OverdraftID,
                               OverdraftID = o.OverdraftID,
                               OverdraftStatus = o.OverdraftStatus,
                               o.OverdraftType,

                               //Employee
                               EmployeeID = o.EmployeeID,
                               //EmployeeCode = Int32.Parse(o.OverdraftStatus == OverdraftStatus.None ? o.Employee.Code : o.HistoricEmployee.Code),
                               //EmployeeDailySalary = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.DailySalary : o.HistoricEmployee.DailySalary,
                               //EmployeeJobPosition = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.JobPosition.Name : o.HistoricEmployee.JobPositionDescription,
                               //EmployeeName = o.OverdraftStatus == OverdraftStatus.None ?
                               //  o.Employee.FirstLastName + (!string.IsNullOrEmpty(o.Employee.SecondLastName) ? (" " + o.Employee.SecondLastName) : "") + " " + o.Employee.Name :
                               //  o.HistoricEmployee.FirstLastName + (!string.IsNullOrEmpty(o.HistoricEmployee.SecondLastName) ? (" " + o.HistoricEmployee.SecondLastName) : "") + " " + o.HistoricEmployee.Name,

                               //Totals
                               //TotalPerceptions = o.OverdraftDetails.Where(x => x.ConceptPayment.ConceptType == ConceptType.SalaryPayment && x.ConceptPayment.Kind == false).Sum(x => x.Amount),
                               //TotalDeductions = o.OverdraftDetails.Where(x => x.ConceptPayment.ConceptType == ConceptType.DeductionPayment && x.ConceptPayment.Kind == false).Sum(x => x.Amount),
                               //TotalLiabilities = o.OverdraftDetails.Where(x => x.ConceptPayment.ConceptType == ConceptType.LiabilityPayment && x.ConceptPayment.Kind == false).Sum(x => x.Amount),
                               
                               TotalPerceptions = o.TotalPerceptions,
                               TotalDeductions = o.TotalDeductions,
                               TotalLiabilities = o.TotalLiabilities,

                               UUID = o.UUID == Guid.Empty ? (Guid?)null : o.UUID

                           }),

                DHData = (from i in indicentsResult.Result.AsParallel()
                          select new
                          {
                              i.ID,
                              i.EmployeeID,
                              i.Date,
                              i.Value,
                              i.IncidentType.Code,
                              i.IncidentType.Name,
                              i.IncidentType.TypeOfIncident
                          }).ToList()
            };

            //Transform vacations
            for (var i = 0; i < vacationsResult.Result.Count(); i++)
            {
                var v = vacationsResult.Result[i];
                for (var j = v.InitialDate; j <= v.FinalDate; j = j.AddDays(1))
                {
                    //Check if day is dayoff
                    if (v.VacationDaysOff.Where(x => x.Date == j).Count() > 0) { continue; }

                    result.DHData.Add(new
                    {
                        ID = v.ID,
                        EmployeeID = v.EmployeeID,
                        Date = j,
                        Value = (decimal)1,
                        Code = "VAC",
                        Name = "Vacaciones",
                        TypeOfIncident = TypeOfIncident.Days,
                    });
                }
            }

            //Transform inahbilities
            for (var i = 0; i < inhabilitiesResult.Result.Count(); i++)
            {
                var inh = inhabilitiesResult.Result[i];
                for (int j = 0; j < inh.AuthorizedDays; j++)
                {
                    result.DHData.Add(new
                    {
                        ID = inh.ID,
                        EmployeeID = inh.EmployeeID,
                        Date = inh.InitialDate.AddDays(j),
                        Value = (decimal)1,
                        Code = "INC",
                        Name = inh.IncidentType.Name,
                        TypeOfIncident = TypeOfIncident.Days,
                    });
                }
            }

            return Json(result);

            //////Get grid columns (concepts marked as global automatic)
            ////var conceptsResult = conceptsClient.FindAsync(x =>
            ////    x.InstanceID == SessionModel.InstanceID &&
            ////    x.company == SessionModel.CompanyID &&
            ////    x.ConceptType != ConceptType.LiabilityPayment &&
            ////    x.Active &&
            ////    x.GlobalAutomatic &&
            ////    !x.Kind,
            ////    SessionModel.CompanyID, new String[] { });

            //////Get periodDetails
            ////var periodDetailResult = clientPD.FindAsync(x =>
            ////    x.InstanceID == SessionModel.InstanceID &&
            ////    x.company == SessionModel.CompanyID &&
            ////    x.ID == periodDetailID &&
            ////    x.Active,
            ////    SessionModel.CompanyID, new String[] { });

            //////Get the overdrafts
            ////var overdraftsResult = clientO.FindAsync(x =>
            ////    x.InstanceID == SessionModel.InstanceID &&
            ////    x.company == SessionModel.CompanyID &&
            ////    x.PeriodDetailID == periodDetailID &&
            ////    x.Active,
            ////    SessionModel.CompanyID, new String[] { "Employee", "Employee.JobPosition", "OverdraftDetails", "HistoricEmployee" });

            //////Get incidents
            ////var indicentsResult = incidentsClient.FindAsync(x =>
            ////    x.InstanceID == SessionModel.InstanceID &&
            ////    x.company == SessionModel.CompanyID &&
            ////    x.PeriodDetailID == periodDetailID &&
            ////    x.Active,
            ////    SessionModel.CompanyID, new String[] { "IncidentType" });

            ////await Task.WhenAll(conceptsResult, overdraftsResult, indicentsResult, periodDetailResult);

            //////Create the result
            ////var result = new
            ////{
            ////    Employees =
            ////                (from o in overdraftsResult.Result
            ////                 orderby o.Employee.Code.Length, o.Employee.Code
            ////                 select new
            ////                 {
            ////                     ID = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.ID : o.HistoricEmployee.EmployeeID,
            ////                     Code = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.Code : o.HistoricEmployee.Code,
            ////                     DailySalary = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.DailySalary : o.HistoricEmployee.DailySalary,
            ////                     JobPositionName = o.OverdraftStatus == OverdraftStatus.None ? o.Employee.JobPosition.Name : o.HistoricEmployee.JobPositionDescription,
            ////                     FullName = o.OverdraftStatus == OverdraftStatus.None ?
            ////                            o.Employee.FirstLastName + (!string.IsNullOrEmpty(o.Employee.SecondLastName) ? (" " + o.Employee.SecondLastName) : "") + " " + o.Employee.Name :
            ////                            o.HistoricEmployee.FirstLastName + (!string.IsNullOrEmpty(o.HistoricEmployee.SecondLastName) ? (" " + o.HistoricEmployee.SecondLastName) : "") + " " + o.HistoricEmployee.Name,
            ////                     o.OverdraftStatus
            ////                 }).ToList(),

            ////    DPOColumns = (from concept in conceptsResult.Result
            ////                  orderby concept.ConceptType, concept.Code
            ////                  select new
            ////                  {
            ////                      ConceptPaymentID = concept.ID,
            ////                      field = "F_" + concept.ID.ToString().Replace("-", ""),
            ////                      title = concept.Name,
            ////                  }).ToList(),

            ////    DPOData =
            ////            (from o in overdraftsResult.Result
            ////             orderby o.Employee.Code
            ////             select new
            ////             {
            ////                 o.ID,
            ////                 o.EmployeeID,
            ////                 UUID = o.UUID == Guid.Empty ? (Guid?)null : o.UUID,
            ////                 Details = (from d in o.OverdraftDetails
            ////                            select new
            ////                            {
            ////                                d.ConceptPaymentID,
            ////                                d.Amount,
            ////                                //Amount = random.Next(1, 9999),
            ////                                d.IsValueCapturedByUser,
            ////                                d.IsGeneratedByPermanentMovement,
            ////                            }).ToList()
            ////             }).ToList(),

            ////    DHData = (from i in indicentsResult.Result
            ////              select new
            ////              {
            ////                  //i.ID,
            ////                  i.EmployeeID,
            ////                  i.Date,
            ////                  i.Value,
            ////                  i.IncidentType.Code,

            ////              }).ToList()
            ////};

            ////return Json(result);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetIncidents(Guid employeeID, Guid periodDetailID, DateTime incidentDate)
        {
            var findResult = await incidentsClient.FindAsync(
                x =>
                x.InstanceID == SessionModel.InstanceID &&
                x.EmployeeID == employeeID && x.PeriodDetailID == periodDetailID && x.Date.Date == incidentDate.Date,
                SessionModel.CompanyID, new String[] { "IncidentType" });

            var result = (from i in findResult
                          orderby i.IncidentType.Code
                          select new
                          {
                              //i.ID,
                              i.Value,
                              i.IncidentTypeID
                          }).ToList();

            return Json(result);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> SaveIncidents(List<Incident> incidents, Guid employeeID, Guid periodDetailID, DateTime capturedOnDate)
        {
            var incidentsList = new List<Incident>();
            var incidentsDays = new List<DateTime>();

            for (int i = 0; i < incidents.Count(); i++)
            {
                var incident = new Incident()
                {
                    ID = Guid.NewGuid(),
                    Name = String.Empty,
                    Description = String.Empty,
                    PeriodDetailID = incidents[i].PeriodDetailID,
                    IncidentTypeID = incidents[i].IncidentTypeID,
                    EmployeeID = incidents[i].EmployeeID,
                    Value = incidents[i].Value,
                    Date = incidents[i].Date.Date,

                    //Common
                    InstanceID = SessionModel.InstanceID,
                    CompanyID = SessionModel.CompanyID,
                    Active = true,
                    CreationDate = DateTime.Now,
                    StatusID = 1,
                    user = SessionModel.IdentityID,
                    Timestamp = DateTime.Now
                };

                incidentsDays.Add(incidents[i].Date.Date);
                incidentsList.Add(incident);
            }

            //Order the dates
            incidentsDays = incidentsDays.OrderBy(x => x.Date).ToList();

            if (incidentsList.Count() > 0)
            {
                var minDate = incidentsDays.First().Date;
                var maxDate = incidentsDays.Last().Date;

                //Just delete incidents of days
                await incidentsClient.DeleteByExpresssionAsync(x =>
                x.EmployeeID == employeeID &&
                x.PeriodDetailID == periodDetailID &&
                x.Date.Date >= minDate && x.Date.Date <= maxDate &&
                x.InstanceID == SessionModel.InstanceID && x.Active == true,
                SessionModel.CompanyID);

                //Save again
                await incidentsClient.CreateAsync(incidentsList, SessionModel.CompanyID);
            }
            else if (incidentsList.Count() == 0)
            {
                var date = capturedOnDate.Date;

                //Just delete incidents of days
                await incidentsClient.DeleteByExpresssionAsync(x =>
                x.EmployeeID == employeeID &&
                x.PeriodDetailID == periodDetailID &&
                x.Date.Date == date &&
                x.InstanceID == SessionModel.InstanceID && x.Active == true,
                SessionModel.CompanyID);

                //fire and forget calculation
                var calculationFireAndForgetParams = new CalculationFireAndForgetByEmployeeParams()
                {
                    EmployeeIds = new List<Guid> { employeeID },
                    IdentityWorkID = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    UserID = SessionModel.IdentityID
                };
                await calculationClient.CalculationFireAndForgetByEmployeesAsync(calculationFireAndForgetParams);

            }

            return Json("OK");
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Authorize(Guid periodDetailID)
        {
            var authorizationParams = new AuthorizationParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                PeriodDetailIDToAuthorize = periodDetailID,
                user = SessionModel.IdentityID
            };
            await authorizeClient.AuthorizationAsync(authorizationParams);

            return Json("OK");
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> Stamping(List<Guid> overdrafstList, Guid periodDetailID)
        {
            try
            {
                Trace.WriteLine("Llega al controller");
                if (!overdrafstList.Any())
                {
                    throw new CotorraException(102, "102", "No existe ninguna nómina a timbrar.", null);
                }

                var detailsList = new List<PayrollStampingDetail>();
                for (int i = 0; i < overdrafstList.Count(); i++)
                {
                    var odID = overdrafstList[i];
                    detailsList.Add(new PayrollStampingDetail()
                    {
                        Folio = String.Empty,
                        Series = String.Empty,
                        PaymentDate = DateTime.Now,
                        RFCOriginEmployer = String.Empty,
                        OverdraftID = odID,
                    });
                }

                var stampingParams = new PayrollStampingParams
                {
                    Detail = detailsList,
                    Currency = Currency.MXN,
                    FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12,
                    PeriodDetailID = periodDetailID,
                    IdentityWorkID = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                };
                Trace.WriteLine("Invoka al cliente");
                var stampProcess = await stampingClient.PayrollStampingAsync(stampingParams);
                var result = from psr in stampProcess.PayrollStampingResultDetails
                             select new
                             {
                                 psr.HistoricEmployeeID,
                                 psr.OverdraftID,
                                 psr.PeriodDetailID,
                                 psr.UUID,
                                 psr.PayrollStampingResultStatus,
                                 psr.Message
                             };

                return Json(result);
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.ToString());
                throw;
            }
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetSummary(Guid periodDetailID)
        {
            var findResult = await clientOD.FindAsync(
                x => x.InstanceID == SessionModel.InstanceID &&
                x.Overdraft.PeriodDetailID == periodDetailID &&
                x.Active,
                SessionModel.CompanyID, new String[] { "ConceptPayment" });

            var result = findResult
                .OrderBy(x => x.ConceptPayment.ConceptType)
                .ThenBy(x => x.ConceptPayment.Code)
                .GroupBy(x => x.ConceptPaymentID)
                .Select(x =>
                    new
                    {
                        ID = x.Key,
                        ConceptCode = x.First().ConceptPayment.Code,
                        ConceptName = x.First().ConceptPayment.Name,
                        ConceptType = x.First().ConceptPayment.ConceptType,
                        ConceptKind = x.First().ConceptPayment.Kind,

                        //Total perceptions
                        TotalPerceptions =
                            x.First().ConceptPayment.ConceptType == ConceptType.SalaryPayment ?
                            x.Sum(x => x.Amount) : (Decimal?)null,

                        //Total deductions
                        TotalDeductions =
                            x.First().ConceptPayment.ConceptType == ConceptType.DeductionPayment ?
                            x.Sum(x => x.Amount) : (Decimal?)null,

                        //Total liabilities
                        TotalLiabilities =
                            x.First().ConceptPayment.ConceptType == ConceptType.LiabilityPayment ?
                            x.Sum(x => x.Amount) : (Decimal?)null,

                        //Total perceptions
                        TotalAmount1 =
                            x.First().ConceptPayment.ConceptType == ConceptType.SalaryPayment ?
                            x.Sum(x => x.Taxed) : (Decimal?)null,

                        //Total perceptions
                        TotalAmount2 =
                            x.First().ConceptPayment.ConceptType == ConceptType.SalaryPayment ?
                            x.Sum(x => x.Exempt) : (Decimal?)null,

                        //Total perceptions
                        TotalAmount3 =
                            x.First().ConceptPayment.ConceptType == ConceptType.SalaryPayment ?
                            x.Sum(x => x.IMSSTaxed) : (Decimal?)null,

                        //Total perceptions
                        TotalAmount4 =
                            x.First().ConceptPayment.ConceptType == ConceptType.SalaryPayment ?
                            x.Sum(x => x.IMSSExempt) : (Decimal?)null,

                    }
                );

            return Json(result);

        }
    }
}
