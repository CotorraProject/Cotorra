using System;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cotorra.Schema;
using Cotorra.Client;
using System.Linq;
using Cotorra.Web.Utils;
using System.Globalization;
using static Cotorra.Web.Controllers.EmployeesController;
using CotorraNode.Common.Base.Schema;
using Cotorra.Core.Utils;

namespace Cotorra.Web.Controllers
{
    [Authentication]
    public class HistoricEmployeesController : Controller
    {
        private readonly Client<HistoricEmployee> client;
        private readonly Client<EmployerRegistration> _clientEmployerRegistration;
        private readonly ClientConfiguration.ClientAdapter clientAdapter;

        public HistoricEmployeesController()
        {
            SessionModel.Initialize();
            clientAdapter = ClientConfiguration.GetAdapterFromConfig();
            var authorization = SessionModel.AuthorizationHeader;
            client = new Client<HistoricEmployee>(authorization, clientAdapter);
            _clientEmployerRegistration = new Client<EmployerRegistration>(authorization, clientAdapter);
        }

        [HttpGet]
        [TelemetryUI]
        public async Task<JsonResult> Get()
        {
            var employees = await client.GetAllAsync(SessionModel.CompanyID, SessionModel.InstanceID);
          
            var result = from e in employees.AsParallel()
                         select new
                         {
                             Avatar = EmployeesController.getGenderName(e.Gender) + RandomSecure.RandomIntFromRNG(1, 6).ToString() + "_sm.png",
                             e.ID,
                             e.Active,
                             e.Code,
                             e.EntryDate,
                             RFC = e.RFC.ToUpper(),
                             NSS = e.NSS.ToUpper(),
                             CURP = e.CURP.ToUpper(),
                             e.PeriodTypeID,
                             e.DailySalary,
                             //e.SBC,
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
                             e.UMF,
                             e.ZipCode,
                             e.CLABE,
                             e.BankBranchNumber,
                             e.IdentityUserID,
                             IsIdentityVinculated = e.IdentityUserID != null && e.IdentityUserID != Guid.Empty,
                             e.LastStatusChange,
                             e.LocalStatus,
                             e.BenefitType,
                             e.ImmediateLeaderEmployeeID,
                             e.DepartmentDescription,
                             e.EmployerRegistrationDescription,
                             e.JobPositionDescription,
                             e.PeriodTypeDescription,
                             e.WorkshiftDescription
                         };


            return Json(result.OrderBy(p => p.Code.Length).ThenBy(p => p.Code));
        }

        [HttpPut]
        [TelemetryUI]
        public async Task<JsonResult> Save(EmployeeDTO data)
        {
            var lstEmployee = new List<HistoricEmployee>();
            
            Guid? identityUserId = null;

            var employerRegistrationTuple = await GetEmployerRegistrationDescription(data.EmployerRegistrationID, SessionModel.CompanyID, SessionModel.InstanceID);

            string employerRegistrationDescription = employerRegistrationTuple.Item2;
            string employerRegistrationCode = employerRegistrationTuple.Item1;
            string employerRegistrationFederalEntity = employerRegistrationTuple.Item3;

            string periodTypeDescription = await GetDescriptionFromName(new Client<PeriodType>(SessionModel.AuthorizationHeader,
                 clientadapter: clientAdapter), data.PeriodTypeID, SessionModel.CompanyID);

            string workshiftDescription = await GetDescriptionFromName(new Client<Workshift>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter), data.WorkShiftID, SessionModel.CompanyID);

            string jobPositionDescription = await GetDescriptionFromName(new Client<JobPosition>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter), data.JobPositionID, SessionModel.CompanyID);

            string departmentDescription = await GetDescriptionFromName(new Client<Department>(SessionModel.AuthorizationHeader,
                clientadapter: clientAdapter), data.DepartmentID, SessionModel.CompanyID);

            HistoricEmployee historicEmployee = new HistoricEmployee();
            if (data.ID.HasValue)
            {
                var IDHistoric = data.ID ?? Guid.NewGuid();

                historicEmployee = (await client.FindAsync(x => x.ID == IDHistoric && x.Active, SessionModel.CompanyID)).FirstOrDefault();
            }


            DateTime? defaultDate = null;
            historicEmployee.ID = data.ID ?? Guid.NewGuid();
            historicEmployee.Active = true;
            historicEmployee.company = SessionModel.CompanyID;
            historicEmployee.InstanceID = SessionModel.InstanceID;
            historicEmployee.CreationDate = DateTime.Now;
            historicEmployee.StatusID = 1;
            historicEmployee.user = SessionModel.IdentityID;
            historicEmployee.Timestamp = DateTime.Now;
            historicEmployee.BirthDate = String.IsNullOrEmpty(data.BirthDate) ? defaultDate : DateTime.ParseExact(data.BirthDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            historicEmployee.BankAccount = data.BankAccount;
            historicEmployee.CivilStatus = data.CivilStatus;
            historicEmployee.Code = data.Code;
            historicEmployee.ContractType = data.ContractType;
            historicEmployee.ContributionBase = data.ContributionBase;
            historicEmployee.CURP = data.CURP;
            historicEmployee.DailySalary = data.DailySalary;
            historicEmployee.DepartmentID = data.DepartmentID;
            historicEmployee.Description = "desc";
            historicEmployee.Email = data.Email;
            historicEmployee.EmployerRegistrationID = data.EmployerRegistrationID;
            historicEmployee.BankID = data.BankID == null || data.BankID == Guid.Empty ? null : data.BankID;
            historicEmployee.EntryDate = DateTime.ParseExact(data.EntryDate, "dd/MM/yyyy", CultureInfo.InvariantCulture);
            historicEmployee.ExteriorNumber = data.ExteriorNumber;
            historicEmployee.FederalEntity = data.FederalEntity;
            historicEmployee.FirstLastName = data.FirstLastName;
            historicEmployee.Gender = data.Gender;
            historicEmployee.InteriorNumber = data.InteriorNumber;
            historicEmployee.JobPositionID = data.JobPositionID;
            historicEmployee.Municipality = data.Municipality;
            historicEmployee.Name = data.Name;
            historicEmployee.NSS = data.NSS;
            historicEmployee.PaymentBase = data.PaymentBase;
            historicEmployee.PaymentMethod = data.PaymentMethod;
            historicEmployee.PeriodTypeID = data.PeriodTypeID;
            historicEmployee.Phone = data.Phone;
            historicEmployee.RegimeType = data.RegimeType;
            historicEmployee.RFC = data.RFC;
            historicEmployee.SalaryZone = data.SalaryZone;
            historicEmployee.SBCFixedPart = data.SBCFixedPart;
            historicEmployee.SBCMax25UMA = data.SBCMax25UMA;
            historicEmployee.SBCVariablePart = data.SBCVariablePart;
            historicEmployee.SecondLastName = data.SecondLastName;
            historicEmployee.Street = data.Street;
            historicEmployee.Suburb = data.Suburb;
            historicEmployee.UMF = data.UMF;
            historicEmployee.WorkshiftID = data.WorkShiftID;
            historicEmployee.ZipCode = data.ZipCode;
            historicEmployee.EmployeeTrustLevel = data.EmployeeTrustLevel == 1 ? EmployeeTrustLevel.Unionized : EmployeeTrustLevel.Trusted;
            historicEmployee.CLABE = data.CLABE;
            historicEmployee.BankBranchNumber = data.BankBranchNumber;
            historicEmployee.IdentityUserID = identityUserId;
            historicEmployee.LastStatusChange = data.ID.HasValue ? data.LastStatusChange : DateTime.Now;
            historicEmployee.LocalStatus = (CotorriaStatus)data.LocalStatus;
            historicEmployee.BenefitType = data.BenefitType;
            historicEmployee.ImmediateLeaderEmployeeID = data.ImmediateLeaderEmployeeID;
            historicEmployee.EmployerRegistrationDescription = employerRegistrationDescription;
            historicEmployee.EmployerRegistrationCode = employerRegistrationCode;
            historicEmployee.EmployerRegistrationFederalEntity = employerRegistrationFederalEntity;
            historicEmployee.PeriodTypeDescription = periodTypeDescription;
            historicEmployee.WorkshiftDescription = workshiftDescription;
            historicEmployee.JobPositionDescription = jobPositionDescription;
            historicEmployee.DepartmentDescription = departmentDescription;

            lstEmployee.Add(historicEmployee);

            if (!data.ID.HasValue)
            {
                await client.CreateAsync(lstEmployee, SessionModel.CompanyID);
            }
            else
            {
                await client.UpdateAsync(lstEmployee, SessionModel.CompanyID);
            }

            return Json(new
            {
                lstEmployee.FirstOrDefault().ID,
                Avatar = EmployeesController.getGenderName(data.Gender) + RandomSecure.RandomIntFromRNG(1, 6).ToString() + "_sm.png",
                lstEmployee.FirstOrDefault().FullName
            });
        }


        private async Task< (string, string, string)> GetEmployerRegistrationDescription(Guid? idToFind, Guid companyID, Guid instanceID)
        {           
            var res = (await _clientEmployerRegistration.FindAsync(x => x.ID == idToFind && x.Active && x.InstanceID == instanceID, companyID, new string[]{
                "Address" })).FirstOrDefault();
            if (res != null)
            {
                return (res.Code, res.Name, res.Address?.FederalEntity);
            }
            return  ("", "", "");
        }

        private async Task<string> GetDescriptionFromName<T>(Client<T> client, Guid idToFind, Guid companyID) where T : CatalogEntity
        {
            var res = (await client.FindAsync(x => x.ID == idToFind && x.Active, companyID)).FirstOrDefault();
            if (res != null)
            {
                return res.Name;
            }
            return "";
        }
    }
}
