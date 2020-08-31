using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Runtime.Serialization;
using Cotorra.Core.Utils;
using CommonUX = CotorraNode.App.Common.UX;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class EmployeesController : Controller
    {
        private readonly Client<Employee> client;
        private readonly EmployeeIdentityRegistrationClient employeeIdentityRegistrationClient;
        private readonly Client<EmployeeSalaryIncrease> _clientEmployeeSalary;
        private readonly Client<EmployeeSBCAdjustment> _clientEmployeeSBCAdjustment;
        private readonly Client<PeriodDetail> clientPD;
        private readonly CalculationClient _calculationClient;

        public EmployeesController()
        {
            SessionModel.Initialize();
            var clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            client = new Client<Employee>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            employeeIdentityRegistrationClient = new EmployeeIdentityRegistrationClient(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            _clientEmployeeSalary = new Client<EmployeeSalaryIncrease>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            clientPD = new Client<PeriodDetail>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            _calculationClient = new CalculationClient(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
            _clientEmployeeSBCAdjustment = new Client<EmployeeSBCAdjustment>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter);
        }

        /// <summary>
        /// Get IdentityID from employee
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>

        /// <summary>
        /// Obtiene la identidad a partir del email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetIdentityUser(string email)
        {
            var result = await employeeIdentityRegistrationClient.GetIdentityUserAsync(email);
            return new JsonResult(result);
        }

        public static string getGenderName(Gender gender) =>
        gender switch
        {
            Gender.Female => "Female",
            Gender.Male => "male",
            0 => "male", //esto es porke no se está guardando el género de forma correcta
            _ => throw new ArgumentException(message: "el genero del empleado proporcionado no es válido", paramName: nameof(gender)),
        };

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            DateTime now = DateTime.Now;

            var employees = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
            var result = from e in employees.AsParallel()
                         select new
                         {
                             Avatar = getGenderName(e.Gender) + Core.Utils.RandomSecure.RandomIntFromRNG(1, 6).ToString() + "_sm.png",
                             e.ID,
                             e.Active,
                             Code = Int32.Parse(e.Code),
                             e.EntryDate,
                             RFC = e.RFC.ToUpper(),
                             NSS = e.NSS.ToUpper(),
                             CURP = e.CURP.ToUpper(),
                             e.PeriodTypeID,
                             e.DailySalary,
                             e.SBCMax25UMA,
                             e.SBCFixedPart,
                             e.SBCVariablePart,
                             e.DepartmentID,
                             e.JobPositionID,
                             e.PaymentMethod,
                             WorkShiftID = e.WorkshiftID,
                             e.BankID,
                             e.SalaryZone,
                             e.RegimeType,
                             e.BankAccount,
                             e.BirthDate,
                             e.BornPlace,
                             e.Cellphone,
                             e.CivilStatus,
                             e.ContractType,
                             e.ContributionBase,
                             e.Description,
                             e.Email,
                             e.EmployerRegistrationID,
                             e.ExteriorNumber,
                             e.FederalEntity,
                             e.FullName,
                             e.FirstLastName,
                             e.Gender,
                             e.InteriorNumber,
                             e.Municipality,
                             e.Name,
                             e.PaymentBase,
                             e.EmployeeTrustLevel,
                             e.Phone,
                             e.SecondLastName,
                             e.Street,
                             e.Suburb,
                             e.Reference,
                             e.UMF,
                             e.ZipCode,
                             e.CLABE,
                             e.BankBranchNumber,
                             e.IdentityUserID,
                             IsIdentityVinculated = e.IdentityUserID != null && e.IdentityUserID != Guid.Empty,
                             e.LastStatusChange,
                             LocalStatus = (int)e.LocalStatus,
                             e.BenefitType,
                             e.ImmediateLeaderEmployeeID,
                             Antiquity = new CalculateDateDifference(now, e.EntryDate).ToStringColaboratorSpanish(),
                             e.IsKioskEnabled,
                         };


            return Json(result.OrderBy(p => p.Code));
        }


        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetByID([FromQuery] Guid ID)
        {
            DateTime now = DateTime.Now;

            var e = (await client.FindAsync(x => x.ID == ID && x.InstanceID == SessionModel.InstanceID && x.Active, SessionModel.CompanyID)).FirstOrDefault();
           
            var result = new
            {
                Avatar = getGenderName(e.Gender) + RandomSecure.RandomIntFromRNG(1, 6).ToString() + "_sm.png",
                e.ID,
                e.Active,
                e.Code,
                e.EntryDate,
                RFC = e.RFC.ToUpper(),
                NSS = e.NSS.ToUpper(),
                CURP = e.CURP.ToUpper(),
                e.PeriodTypeID,
                e.DailySalary,
                e.SBCMax25UMA,
                e.SBCFixedPart,
                e.SBCVariablePart,
                e.DepartmentID,
                e.JobPositionID,
                e.PaymentMethod,
                WorkShiftID = e.WorkshiftID,
                e.BankID,
                e.SalaryZone,
                e.RegimeType,
                e.BankAccount,
                e.BirthDate,
                e.BornPlace,
                e.Cellphone,
                e.CivilStatus,
                e.ContractType,
                e.ContributionBase,
                e.Description,
                e.Email,
                e.EmployerRegistrationID,
                e.ExteriorNumber,
                e.FederalEntity,
                e.FullName,
                e.FirstLastName,
                e.Gender,
                e.InteriorNumber,
                e.Municipality,
                e.Name,
                e.PaymentBase,
                e.EmployeeTrustLevel,
                e.Phone,
                e.SecondLastName,
                e.Street,
                e.Suburb,
                e.Reference,
                e.UMF,
                e.ZipCode,
                e.CLABE,
                e.BankBranchNumber,
                e.IdentityUserID,
                IsIdentityVinculated = e.IdentityUserID != null && e.IdentityUserID != Guid.Empty,
                e.LastStatusChange,
                LocalStatus = (int)e.LocalStatus,
                e.BenefitType,
                e.ImmediateLeaderEmployeeID,
                Antiquity = new CalculateDateDifference(now, e.EntryDate).ToStringColaboratorSpanish(),
                AntiquityForBenefits = new CalculateDateDifference(now, e.EntryDate).Years + 1,
                e.IsKioskEnabled
            };

            return Json(result);
        }


        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(EmployeeDTO data)
        {
            var lstEmployee = new List<Employee>();
           
            var isNew = !data.ID.HasValue;
            Guid? identityUserId = null;
            DateTime now = DateTime.Now;
            CalculateDateDifference calculateDateDifference = new CalculateDateDifference();

            if (isNew)
            {
                lstEmployee.Add(new Employee()
                {
                    ID = Guid.NewGuid(),

                    //Common
                    Active = true,
                    company = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    CreationDate = DateTime.Now,
                    StatusID = 1,
                    user = SessionModel.IdentityID,
                    Timestamp = DateTime.Now,
                    BirthDate = String.IsNullOrEmpty(data.BirthDate) ? (DateTime?)null : CotorraTools.ValidateDate(data.BirthDate),
                    BankAccount = data.BankAccount,
                    Phone = String.IsNullOrEmpty(data.Phone) ? "" : data.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                    Cellphone = String.IsNullOrEmpty(data.Cellphone) ? "" : data.Cellphone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", ""),
                    CivilStatus = data.CivilStatus,
                    Code = data.Code,
                    ContractType = data.ContractType,
                    ContributionBase = data.ContributionBase,
                    CURP = data.CURP,
                    DailySalary = data.DailySalary,
                    DepartmentID = data.DepartmentID,
                    Description = "desc",
                    Email = data.Email,
                    EmployerRegistrationID = data.EmployerRegistrationID,
                    BankID = data.BankID == null || data.BankID == Guid.Empty ? null : data.BankID,
                    EntryDate = CotorraTools.ValidateDate(data.EntryDate),
                    ExteriorNumber = data.ExteriorNumber,
                    FederalEntity = data.FederalEntity,
                    FirstLastName = data.FirstLastName,
                    Gender = data.Gender,
                    InteriorNumber = data.InteriorNumber,
                    JobPositionID = data.JobPositionID,
                    Municipality = data.Municipality,
                    Name = data.Name,
                    NSS = String.IsNullOrEmpty(data.NSS) ? "" : data.NSS,
                    PaymentBase = data.PaymentBase,
                    PaymentMethod = data.PaymentMethod,
                    PeriodTypeID = data.PeriodTypeID,
                    RegimeType = data.RegimeType,
                    RFC = data.RFC,
                    SalaryZone = data.SalaryZone,
                    SBCFixedPart = data.SBCFixedPart,
                    SBCMax25UMA = data.SBCMax25UMA,
                    SBCVariablePart = data.SBCVariablePart,
                    SecondLastName = String.IsNullOrEmpty(data.SecondLastName) ? "" : data.SecondLastName,
                    Street = data.Street,
                    Suburb = data.Suburb,
                    Reference = data.Reference,
                    UMF = data.UMF,
                    WorkshiftID = data.WorkShiftID,
                    ZipCode = data.ZipCode,
                    EmployeeTrustLevel = data.EmployeeTrustLevel == 1 ? EmployeeTrustLevel.Unionized : EmployeeTrustLevel.Trusted,
                    CLABE = data.CLABE,
                    BankBranchNumber = data.BankBranchNumber,
                    IdentityUserID = identityUserId,
                    LastStatusChange = data.ID.HasValue ? data.LastStatusChange : DateTime.Now,
                    LocalStatus = (CotorriaStatus)data.LocalStatus,
                    BenefitType = data.BenefitType,
                    ImmediateLeaderEmployeeID = data.ImmediateLeaderEmployeeID,                  
                });

                await client.CreateAsync(lstEmployee, new LicenseParams()
                {
                    IdentityWorkID = SessionModel.CompanyID,
                    LicenseServiceID = SessionModel.LicenseServiceID,
                    LicenseID = SessionModel.LicenseID,
                    AuthTkn = SessionModel.AuthorizationHeader
                });
            }
            else
            {
                var result = await client.GetByIdsAsync(new List<Guid> { data.ID.Value }, SessionModel.CompanyID);
                var employeeToUpdate = result.FirstOrDefault();

                if (data.UpdateType == "PersonalData")
                {
                    employeeToUpdate.FirstLastName = data.FirstLastName;
                    employeeToUpdate.SecondLastName = data.SecondLastName;
                    employeeToUpdate.Name = data.Name;
                    employeeToUpdate.RFC = data.RFC;
                    employeeToUpdate.CURP = data.CURP;
                    employeeToUpdate.BirthDate = String.IsNullOrEmpty(data.BirthDate) ? (DateTime?)null : CotorraTools.ValidateDate(data.BirthDate);
                    employeeToUpdate.Gender = data.Gender;
                    employeeToUpdate.CivilStatus = data.CivilStatus;
                    employeeToUpdate.Phone = String.IsNullOrEmpty(data.Phone) ? "" : data.Phone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    employeeToUpdate.Cellphone = String.IsNullOrEmpty(data.Cellphone) ? "" : data.Cellphone.Replace("(", "").Replace(")", "").Replace("-", "").Replace(" ", "");
                    employeeToUpdate.Email = data.Email;
                }
                else if (data.UpdateType == "Address")
                {
                    employeeToUpdate.ZipCode = data.ZipCode;
                    employeeToUpdate.FederalEntity = data.FederalEntity;
                    employeeToUpdate.Municipality = data.Municipality;
                    employeeToUpdate.Street = data.Street;
                    employeeToUpdate.ExteriorNumber = data.ExteriorNumber;
                    employeeToUpdate.InteriorNumber = data.InteriorNumber;
                    employeeToUpdate.Suburb = data.Suburb;
                    employeeToUpdate.Reference = data.Reference;
                }
                else if (data.UpdateType == "Employment")
                {
                    employeeToUpdate.Code = data.Code;
                    employeeToUpdate.EntryDate = CotorraTools.ValidateDate(data.EntryDate);
                    employeeToUpdate.ContractType = data.ContractType;
                    employeeToUpdate.RegimeType = data.RegimeType;
                    employeeToUpdate.PeriodTypeID = data.PeriodTypeID;
                    employeeToUpdate.WorkshiftID = data.WorkShiftID;
                    employeeToUpdate.EmployeeTrustLevel = (EmployeeTrustLevel)data.EmployeeTrustLevel;
                    employeeToUpdate.BenefitType = data.BenefitType;
                    employeeToUpdate.SalaryZone = data.SalaryZone;
                    employeeToUpdate.DailySalary = data.DailySalary;
                    employeeToUpdate.DepartmentID = data.DepartmentID;
                    employeeToUpdate.JobPositionID = data.JobPositionID;
                    employeeToUpdate.ImmediateLeaderEmployeeID = data.ImmediateLeaderEmployeeID;
                }
                else if (data.UpdateType == "SocialSecurity")
                {
                    employeeToUpdate.NSS = data.NSS;
                    employeeToUpdate.EmployerRegistrationID = data.EmployerRegistrationID;
                    employeeToUpdate.UMF = data.UMF;
                    employeeToUpdate.ContributionBase = data.ContributionBase;
                    employeeToUpdate.SBCFixedPart = data.SBCFixedPart;
                    employeeToUpdate.SBCVariablePart = data.SBCVariablePart;
                    employeeToUpdate.SBCMax25UMA = data.SBCMax25UMA;
                }
                else if (data.UpdateType == "Payment")
                {
                    employeeToUpdate.PaymentMethod = data.PaymentMethod;
                    employeeToUpdate.BankID = data.BankID;
                    employeeToUpdate.BankBranchNumber = data.BankBranchNumber;
                    employeeToUpdate.BankAccount = data.BankAccount;
                    employeeToUpdate.CLABE = data.CLABE;
                }
                else if (data.UpdateType == "Kiosk")
                {
                    employeeToUpdate.IsKioskEnabled = data.IsKioskEnabled;
                    if (data.IsKioskEnabled)
                    {
                        if (!String.IsNullOrEmpty(data.Email))
                        {
                            if (EmailUtil.IsValid(data.Email))
                            {
                                employeeToUpdate.Email = data.Email;
                            }
                            else
                            {
                                throw new CotorraException(1003, "1003", "El Email es incorrecto o está mal formado.", null);
                            }
                        }
                        else
                        {
                            throw new CotorraException(1003, "1003", "El Email es obligatorio, cuando se habilita el uso del Kiosko del Empleado.", null);
                        }
                    }
                }
                else if (data.UpdateType == "Vacations")
                {
                    //Not now
                }

                lstEmployee.Add(employeeToUpdate);

                await client.UpdateAsync(lstEmployee, SessionModel.CompanyID);
                return Json("OK");
            }

            //Recalculate thru salary raise update

            if (isNew || (!isNew && data.UpdateType == "SocialSecurity"))
            {
                var ids = lstEmployee.Select(p => p.ID).ToList();

                await _calculationClient.CalculationFireAndForgetByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
                {
                    EmployeeIds = ids,
                    IdentityWorkID = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    UserID = SessionModel.IdentityID
                });
            }

            return Json(new
            {
                lstEmployee.FirstOrDefault().ID,
                Avatar = getGenderName(data.Gender) + RandomSecure.RandomIntFromRNG(1, 6).ToString() + "_sm.png",
                lstEmployee.FirstOrDefault().FullName,
                Antiquity = calculateDateDifference.Calculate(now, lstEmployee.FirstOrDefault().EntryDate).ToStringColaboratorSpanish(),
            });
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> Delete(Guid id)
        {
            await client.DeleteAsync(new List<Guid>() { id }, new LicenseParams()
            {
                IdentityWorkID = SessionModel.CompanyID,
                LicenseServiceID = SessionModel.LicenseServiceID,
                LicenseID = SessionModel.LicenseID,
                AuthTkn = SessionModel.AuthorizationHeader
            });
            return Json("OK");
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetSalaryUpdateHistory(Guid employeeID)
        {
            var employees = await client.FindAsync(x => x.ID == employeeID && x.Active, SessionModel.CompanyID);
            var employee = employees.FirstOrDefault();

            var periodDetail = (await clientPD.FindAsync(x => x.Period.PeriodTypeID == employee.PeriodTypeID && x.Active 
            && x.InstanceID == SessionModel.InstanceID && x.PeriodStatus == PeriodStatus.Calculating, SessionModel.CompanyID)).FirstOrDefault();

            var initialDate = periodDetail.InitialDate;
            var employeeSalaryIncrease = await _clientEmployeeSalary.FindAsync(x => x.ModificationDate >= initialDate && 
            x.EmployeeID == employeeID &&
           x.Active, SessionModel.CompanyID, new string[] { "EmployeeSBCAdjustment" });

            var result = from esi in employeeSalaryIncrease.AsParallel()
                         select new
                         {
                             esi.ID,
                             ChangeDate = esi.ModificationDate,
                             esi.DailySalary,
                             esi.EmployeeSBCAdjustment.SBCFixedPart,
                             esi.EmployeeSBCAdjustment.SBCVariablePart,
                             esi.EmployeeSBCAdjustment.SBCMax25UMA,
                             esi.EmployeeID,
                             esi.EmployeeSBCAdjustmentID,
                         };
            return Json(result);
        }

        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> UpdateSalary(UpdateSalaryDTO data)
        {
            var periodDetail = await GetActualDetail(data.PeriodTypeID);
            bool updateGrid = false;

            if (DateTime.Parse(data.ChangeDate).Date <= periodDetail.FinalDate &&
                DateTime.Parse(data.ChangeDate).Date >= periodDetail.InitialDate)
            {
                updateGrid = true;
            }

            var ID = data.ID ?? Guid.NewGuid();
            var employeeSBCAdjustmentID = data.EmployeeSBCAdjustmentID ?? Guid.NewGuid();

            var lstEmployeeSalaryIncrement = new List<EmployeeSalaryIncrease>();

            var employeeSBCAdjustment = new EmployeeSBCAdjustment()
            {
                ID = employeeSBCAdjustmentID,
                IdentityID = SessionModel.IdentityID,
                Active = true,
                company = SessionModel.CompanyID,
                CreationDate = DateTime.Now,
                DeleteDate = null,
                Description = " ",
                user = SessionModel.IdentityID,
                InstanceID = SessionModel.InstanceID,
                ModificationDate = DateTime.Parse(data.ChangeDate),
                Name = " ",
                EmployeeID = data.EmployeeID,
                StatusID = 1,
                Timestamp = DateTime.Now,
                SBCFixedPart = data.SBCFixedPart,
                SBCMax25UMA = data.SBCMax25UMA,
                SBCVariablePart = data.SBCVariablePart,
            };

            lstEmployeeSalaryIncrement.Add(new EmployeeSalaryIncrease()
            {
                ID = ID,

                Active = true,
                company = SessionModel.CompanyID,
                CreationDate = DateTime.Now,
                DailySalary = data.DailySalary,
                DeleteDate = null,
                Description = " ",
                user = SessionModel.IdentityID,
                InstanceID = SessionModel.InstanceID,
                ModificationDate = DateTime.Parse(data.ChangeDate),
                Name = " ",
                EmployeeID = data.EmployeeID,
                StatusID = 1,
                Timestamp = DateTime.Now,
                EmployeeSBCAdjustmentID = employeeSBCAdjustmentID,
                EmployeeSBCAdjustment = employeeSBCAdjustment
            });

            if (!data.ID.HasValue)
            {
                await _clientEmployeeSalary.CreateAsync(lstEmployeeSalaryIncrement, SessionModel.CompanyID);
            }
            else
            {
                await _clientEmployeeSalary.UpdateAsync(lstEmployeeSalaryIncrement, SessionModel.CompanyID);
            }

            //recalcular debido a modificación en incremento de salario
            await _calculationClient.CalculationFireAndForgetByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = new List<Guid> { data.EmployeeID },
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });

            return Json(new { ID, EmployeeSBCAdjustmentID = employeeSBCAdjustmentID, UpdateGrid = updateGrid });
        }

        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> DeleteEmployeeSalaryIncrease(Guid id, Guid employeeID)
        {
            var found = (await _clientEmployeeSalary.FindAsync(x => x.Active && x.InstanceID == SessionModel.InstanceID 
                && x.ID == id, SessionModel.CompanyID)).
                FirstOrDefault();

            if (found != null)
            {
                await _clientEmployeeSalary.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);

                var employees = await client.FindAsync(x => x.ID == employeeID && x.Active, SessionModel.CompanyID);
                var employee = employees.FirstOrDefault();

                //recalcular debido a modificación en incremento de salario
                await _calculationClient.CalculationFireAndForgetByEmployeesAsync(
                    new CalculationFireAndForgetByEmployeeParams()
                    {
                        EmployeeIds = new List<Guid> { employeeID },
                        IdentityWorkID = SessionModel.CompanyID,
                        InstanceID = SessionModel.InstanceID,
                        UserID = SessionModel.IdentityID
                    });

                return Json(new { employee.SBCFixedPart, employee.DailySalary, employee.SBCVariablePart, employee.SBCMax25UMA });

            }
            return Json("OK");

        }


        /// <summary>
        /// Obtiene el periodo actual
        /// </summary>
        /// <param name="periodTypeID"></param>
        /// <returns></returns>
        public async Task<PeriodDetail> GetActualDetail(Guid periodTypeID)
        {
            var periodDetailss = (await clientPD.FindAsync(x =>
                   x.Period.PeriodTypeID == periodTypeID &&
                    x.Active &&
                   x.InstanceID == SessionModel.InstanceID &&
                   (x.PeriodStatus == PeriodStatus.Calculating || x.PeriodStatus == PeriodStatus.Authorized),
                   SessionModel.CompanyID, new String[] { "Period" })).OrderByDescending(x => x.Number);


            return periodDetailss.FirstOrDefault();
        }


        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> GetSBCUpdateHistory(Guid employeeID)
        {
            var employeeSalaryIncrease = await _clientEmployeeSBCAdjustment.FindAsync(x => x.EmployeeID == employeeID &&
           x.Active, SessionModel.CompanyID);

            var result = from esi in employeeSalaryIncrease.AsParallel()
                         select new
                         {
                             esi.ID,
                             ChangeDate = esi.ModificationDate,
                             esi.SBCFixedPart,
                             esi.SBCVariablePart,
                             esi.SBCMax25UMA,
                             esi.EmployeeID,
                         };
            return Json(result);
        }


        [HttpPost]
        [TelemetryUI]
        public async Task<JsonResult> UpdateSBC(UpdateSBCDTO data)
        {
            var periodDetail = await GetActualDetail(data.PeriodTypeID);
            bool updateGrid = false;

            if (DateTime.Parse(data.ChangeDate).Date <= periodDetail.FinalDate &&
                DateTime.Parse(data.ChangeDate).Date >= periodDetail.InitialDate)
            {
                updateGrid = true;
            }

            var ID = data.ID ?? Guid.NewGuid();

            var lstEmployeeSalaryIncrement = new List<EmployeeSBCAdjustment>
            {
                new EmployeeSBCAdjustment()
                {
                    ID = ID,

                    Active = true,
                    company = SessionModel.CompanyID,
                    CreationDate = DateTime.Now,
                    DeleteDate = null,
                    Description = " ",
                    user = SessionModel.IdentityID,
                    InstanceID = SessionModel.InstanceID,
                    ModificationDate = DateTime.Parse(data.ChangeDate),
                    Name = " ",
                    EmployeeID = data.EmployeeID,
                    StatusID = 1,
                    Timestamp = DateTime.Now,
                    SBCFixedPart = data.SBCFixedPart,
                    SBCMax25UMA = data.SBCMax25UMA,
                    SBCVariablePart = data.SBCVariablePart,
                }
            };

            if (!data.ID.HasValue)
            {
                await _clientEmployeeSBCAdjustment.CreateAsync(lstEmployeeSalaryIncrement, SessionModel.CompanyID);
            }
            else
            {
                await _clientEmployeeSBCAdjustment.UpdateAsync(lstEmployeeSalaryIncrement, SessionModel.CompanyID);
            }

            //recalcular debido a modificación en incremento de salario
            await _calculationClient.CalculationFireAndForgetByEmployeesAsync(new CalculationFireAndForgetByEmployeeParams()
            {
                EmployeeIds = new List<Guid> { data.EmployeeID },
                IdentityWorkID = SessionModel.CompanyID,
                InstanceID = SessionModel.InstanceID,
                UserID = SessionModel.IdentityID
            });

            return Json(new { ID, UpdateGrid = updateGrid });
        }


        [HttpDelete]
        [TelemetryUI]
        public async Task<JsonResult> DeleteEmployeeSBCAdjustment(Guid id, Guid employeeID)
        {
            await _clientEmployeeSBCAdjustment.DeleteAsync(new List<Guid>() { id }, SessionModel.CompanyID);

            var employees = await client.FindAsync(x => x.ID == employeeID && x.Active, SessionModel.CompanyID);
            var employee = employees.FirstOrDefault();

            //recalcular debido a modificación en incremento de salario
            await _calculationClient.CalculationFireAndForgetByEmployeesAsync(
                new CalculationFireAndForgetByEmployeeParams()
                {
                    EmployeeIds = new List<Guid> { employeeID },
                    IdentityWorkID = SessionModel.CompanyID,
                    InstanceID = SessionModel.InstanceID,
                    UserID = SessionModel.IdentityID
                });

            return Json(new { employee.SBCFixedPart, employee.DailySalary, employee.SBCVariablePart, employee.SBCMax25UMA });
        }

        [DataContract]
        public class EmployeeDTO
        {
            public String UpdateType { get; set; }
            public Guid? ID { get; set; }
            public string Code { get; set; }
            public string EntryDate { get; set; }
            public ContractType ContractType { get; set; }
            public string Name { get; set; }
            public string FirstLastName { get; set; }
            public string SecondLastName { get; set; }
            public Guid PeriodTypeID { get; set; }
            public decimal DailySalary { get; set; }
            public BaseQuotation ContributionBase { get; set; }
            public decimal SBCFixedPart { get; set; }
            public decimal SBCVariablePart { get; set; }
            public decimal SBCMax25UMA { get; set; }
            public Guid DepartmentID { get; set; }
            public Guid JobPositionID { get; set; }
            public Guid? BankID { get; set; }
            public int EmployeeTrustLevel { get; set; }
            public BenefitTypeValue BenefitType { get; set; }
            public PaymentBase PaymentBase { get; set; }
            public PaymentMethod PaymentMethod { get; set; }
            public Guid WorkShiftID { get; set; }
            public SalaryZone SalaryZone { get; set; }
            public string FonacotNumber { get; set; }
            public string AFORE { get; set; }
            public string UMF { get; set; }
            public CivilStatus CivilStatus { get; set; }
            public Gender Gender { get; set; }
            public string BirthDate { get; set; }
            public bool IsForeignWithoutCURP { get; set; }
            public EmployeeRegimeType RegimeType { get; set; }
            public string NSS { get; set; }
            public Guid? EmployerRegistrationID { get; set; }
            public string RFC { get; set; }
            public string CURP { get; set; }
            public string Email { get; set; }
            public string IsIdentityVinculated { get; set; }
            public string BankBranchNumber { get; set; }
            public string BankAccount { get; set; }
            public string CLABE { get; set; }
            public string ZipCode { get; set; }
            public string FederalEntity { get; set; }
            public string Municipality { get; set; }
            public string Street { get; set; }
            public string ExteriorNumber { get; set; }
            public string InteriorNumber { get; set; }
            public string Suburb { get; set; }
            public string Reference { get; set; }
            public string Phone { get; set; }
            public string Cellphone { get; set; }
            public int LocalStatus { get; set; }
            public DateTime LastStatusChange { get; set; }
            public bool IsKioskEnabled { get; set; }
            public Guid? ImmediateLeaderEmployeeID { get; set; }
            /// <summary>
            /// Identidad vinculada al usuario
            /// </summary>
            public Guid? IdentityUserID { get; set; }
        }


        [DataContract]
        public class UpdateSalaryDTO
        {
            public Guid EmployeeID { get; set; }
            public Guid PeriodTypeID { get; set; }
            public Guid? ID { get; set; }
            public Guid? EmployeeSBCAdjustmentID { get; set; }
            public string ChangeDate { get; set; }
            public decimal DailySalary { get; set; }
            public decimal SBCFixedPart { get; set; }
            public decimal SBCVariablePart { get; set; }
            public decimal SBCMax25UMA { get; set; }
        }

        [DataContract]
        public class UpdateSBCDTO
        {
            public Guid EmployeeID { get; set; }
            public Guid PeriodTypeID { get; set; }
            public Guid? ID { get; set; }
            public string ChangeDate { get; set; }
            public decimal SBCFixedPart { get; set; }
            public decimal SBCVariablePart { get; set; }
            public decimal SBCMax25UMA { get; set; }
        }
    }
}
