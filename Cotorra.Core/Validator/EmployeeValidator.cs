using CotorraNode.Common.Config;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Data.SqlClient;
using MoreLinq;
using Cotorra.Core.Managers;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Core.Managers.Graph;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace Cotorra.Core.Validator
{
    public class EmployeeValidator : IValidator<Employee>, IStatusFullValidator<Employee>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Employee> MiddlewareManager { get; set; }
        public EmployeeValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 100, 4001));
            createRules.Add(new SimpleStringRule("FirstLastName", "Apellido Paterno", true, 1, 100, 4001));
            //createRules.Add(new SimpleStringRule("SecondLastName", "Apellido Materno", true, 1, 100, 4001));
            //createRules.Add(new SimpleStringRule("NSS", "Número de Seguro Social", true, 1, 12, 4001));
            createRules.Add(new SimpleStringRule("RFC", "RFC", true, 1, 14, 4001));
            createRules.Add(new SimpleStringRule("CURP", "CURP", true, 1, 19, 4001));
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));

            createRules.Add(new GuidRule("DepartmentID", "ID Departamento", 4002));
            //TODO: Area
            createRules.Add(new GuidRule("JobPositionID", "ID Puesto", 4002));

            createRules.Add(new DuplicateItemRule<Employee>(new string[] { "Code" }, "Código", this, 4004));

            createRules.Add(new SimpleStringRule("Cellphone", "Número de celular", false, 0, 10, 4009));

            ////duplicate keys
            //createRules.Add(new DuplicateItemRule<Employee>(new string[] { "Name", "FirstLastName", "SecondLastName" }, "Nombre completo", this, 4003));
            createRules.Add(new DuplicateItemRule<Employee>(new string[] { "RFC" }, "RFC", this, 4005));
            //createRules.Add(new DuplicateItemRule<Employee>(new string[] { "NSS" }, "NSS", this, 4006));
            createRules.Add(new DuplicateItemRule<Employee>(new string[] { "CURP" }, "CURP", this, 4007));
            //createRules.Add(new DuplicateItemRule<Employee>(new string[] { "CURP" }, "CURP", this, 4008));
            //createRules.Add(new DuplicateItemGlobalRule<Employee>(new string[] { "IdentityUserID" }, "Ya existe un empleado vinculado con esa cuenta de correo", this, 4009));

            ////Foreign keys
            createRules.Add(new ForeignKeyRule<JobPosition>(new string[] { "JobPositionID" }, "Puesto", new JobPositionValidator(), 4010));
        }

        public async Task AfterCreate(List<Employee> lstObjectsToValidate, IParams parameters)
        {
            var overdraftManager = new OverdraftManager();
            overdraftManager.CreateByEmployeesAsync(lstObjectsToValidate).Wait();
            LicenseParams licenseParams = parameters as LicenseParams;

            //Calculation async fire and forget
            IEnumerable<Guid> employeeIds = lstObjectsToValidate.Select(p => p.ID);
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var userId = lstObjectsToValidate.FirstOrDefault().IdentityID;
            new OverdraftCalculationManager().CalculationFireAndForgetByEmployeesAsync(employeeIds, company, instanceID, userId);
            //Updatefeature
            var service = new LicensingService(LicensingServiceProviderFactory.GetProvider());
            await service.ConsumeEmployeeLicense(licenseParams.LicenseID, licenseParams.LicenseServiceID);
        }

        public void AfterCreate(List<Employee> lstObjectsToValidate)
        {
            throw new NotImplementedException("try async :)");
        }

        public async Task AfterDeleteAsync(List<Guid> lstObjectsToValidate, IParams parameters)
        {
            LicenseParams licenseParams = parameters as LicenseParams;

            var service = new LicensingService(LicensingServiceProviderFactory.GetProvider());
            service.AumentEmployeeLicense(licenseParams.LicenseID, licenseParams.LicenseServiceID);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            throw new NotImplementedException("try async :)");
        }

        public void AfterUpdate(List<Employee> lstObjectsToValidate)
        {
            //Employee Identity Registration (Fire and Forget)
            //new EmployeeIdentityRegistrationManager().CreateOrVerifyRegistrationAsync(lstObjectsToValidate.FirstOrDefault());

            //Calculation async fire and forget
            IEnumerable<Guid> employeeIds = lstObjectsToValidate.Select(p => p.ID);
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var userId = lstObjectsToValidate.FirstOrDefault().IdentityID;
            new OverdraftCalculationManager().CalculationFireAndForgetByEmployeesAsync(employeeIds, company, instanceID, userId);
        }

        public void BeforeCreate(List<Employee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Employee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            var nullablelst = lstObjectsToValidate.Cast<Guid?>();
            var employeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = employeeManager.FindByExpressionAsync(p => lstObjectsToValidate.Contains(p.ID) ||
                            nullablelst.Contains(p.ImmediateLeaderEmployeeID), Guid.Empty).Result;

            var employeesToDelete = employees.Where(x => lstObjectsToValidate.Contains(x.ID));
            var employeesToSetLeaderNull = employees.Where(x => nullablelst.Contains(x.ImmediateLeaderEmployeeID));

            if (null != employeesToDelete && employeesToDelete.Any())
            {
                var statusToCheck = new List<OverdraftStatus>() { OverdraftStatus.Authorized, OverdraftStatus.Canceled, OverdraftStatus.Stamped };
                var identityWorkId = employeesToDelete.FirstOrDefault().company;
                var instanceID = employeesToDelete.FirstOrDefault().InstanceID;
                var employeesIds = employeesToDelete.Select(p => p.ID);

                var overdreaftMiddlewareManager = new MiddlewareManager<Overdraft>(new BaseRecordManager<Overdraft>(), new OverdraftValidator());
                var overdrafts = overdreaftMiddlewareManager.FindByExpressionAsync(p => employeesIds.Contains(p.EmployeeID), identityWorkId, new string[] { "OverdraftDetails" }).Result;

                var badOverdrafts = overdrafts.Where(x => statusToCheck.Contains(x.OverdraftStatus));

                if (badOverdrafts != null && badOverdrafts.Any())
                {
                    throw new CotorraException(106, "106", "No es posible eliminar al empleado, ya que tiene nóminas autorizadas timbradas o canceladas. Por favor revisa cambiar el status del empleado en lugar de eliminarlo.", null);
                }
                DeleteEmployeeInformation(lstObjectsToValidate, instanceID, identityWorkId);
            }
        }

        private void DeleteEmployeeInformation(List<Guid> guids, Guid instanceId, Guid identityWorkId)
        {
            using (var connection = new SqlConnection(ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "DeleteEmployeeInformation";


                    DataTable dtGuidList = new DataTable();
                    dtGuidList.Columns.Add("ID", typeof(string));
                    guids.ForEach(p =>
                    {
                        dtGuidList.Rows.Add(p);
                    });
                    SqlParameter param = new SqlParameter("@EmployeeIDs", SqlDbType.Structured)
                    {
                        TypeName = "dbo.guidlisttabletype",
                        Value = dtGuidList
                    };
                    command.Parameters.Add(param);
                    command.Parameters.AddWithValue("@InstanceId", instanceId);
                    command.Parameters.AddWithValue("@company", identityWorkId);

                    //Execute SP
                    command.ExecuteNonQuery();
                }
            }

        }


        public void BeforeUpdate(List<Employee> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Employee>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var identityWorkId = lstObjectsToValidate.Select(p => p.company).FirstOrDefault();
            var lstObjectsIdsToValidate = lstObjectsToValidate.Select(p => p.ID);

            //creates
            var employeeManager = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employeesPrevious = employeeManager.FindByExpressionAsync(p => lstObjectsIdsToValidate.Contains(p.ID), Guid.Empty).Result;

            ConcurrentBag<Employee> employeeChangedPeriod = new ConcurrentBag<Employee>();

            Parallel.ForEach(lstObjectsToValidate, employeeToUpdate =>
            {
                var previous = employeesPrevious.FirstOrDefault(p => p.ID == employeeToUpdate.ID);

                //Si cambia el periodo o cambia la fecha de ingreso se vuelve a crear su overdraft (delete and create)
                if (previous.PeriodTypeID != employeeToUpdate.PeriodTypeID || previous.EntryDate != employeeToUpdate.EntryDate)
                {
                    employeeChangedPeriod.Add(employeeToUpdate);
                }
            });

            //verify if employee changed period
            if (employeeChangedPeriod.Any())
            {
                new OverdraftManager().UpdateByEmployeesAsync(employeeChangedPeriod.ToList()).Wait();
            }
        }


        #region status
        /// <summary> status


        public Task BeforeActivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public Task AfterActivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public async Task BeforeUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            await Task.Delay(1);
        }

        public async Task AfterUnregistered(IEnumerable<Guid> idsToUpdate, Guid identityWorkId, params object[] parameters)
        {
            LicenseParams licenseParams = parameters[0] as LicenseParams;
            var service = new LicensingService(LicensingServiceProviderFactory.GetProvider());
            service.AumentEmployeeLicense(licenseParams.LicenseID, licenseParams.LicenseServiceID);
            //Vacío (corte)
        }

        public Task BeforeInactivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public Task AfterInactivate(IEnumerable<Guid> idsToUpdate, Guid identityWorkId)
        {
            throw new NotImplementedException();
        }

        public string GetPersonalizedQuery(CotorriaStatus status, string keySplitted, params object[] parameters)
        {
            ApplySettlementProcessParams setparams = parameters[0] as ApplySettlementProcessParams;
            if (status == CotorriaStatus.Unregistered)
            {
                string dateUnchange = Convert.ToDateTime(setparams.SettlementEmployeeSeparationDate).ToString("yyyy-MM-dd 00:00:00.00");
                return $"update  Employee set LocalStatus = {(int)status},  LastStatusChange = getdate() , UnregisteredDate = '{dateUnchange}'  where ID in ({keySplitted}) ";
            }
            return string.Empty;
        }



        #endregion
    }
}
