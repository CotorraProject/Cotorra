using CotorraNode.Common.Config;
using Cotorra.Schema;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Cotorra.Core.Validator
{
    public class EmployeeSalaryIncreaseValidator : IValidator<EmployeeSalaryIncrease>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>(); 

        public IMiddlewareManager<EmployeeSalaryIncrease> MiddlewareManager { get; set; }
        public IMiddlewareManager<EmployeeSBCAdjustment> _middlewareManagerSBCAdjustment { get; set; }

        public EmployeeSalaryIncreaseValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
            createRules.Add(new GuidRule("EmployeeID", "ID Empleado", 4002));

            createRules.Add(new DuplicateItemRule<EmployeeSalaryIncrease>(new string[] { "EmployeeID", "ModificationDate" }, "Ya existe un movimiento de incremento de salario para este colaborador en la fecha seleccionada.", this, 4003));

            _middlewareManagerSBCAdjustment = new MiddlewareManager<EmployeeSBCAdjustment>(new BaseRecordManager<EmployeeSBCAdjustment>(), new EmployeeSBCAdjustmentValidator());
        }

        private void BeforeDeleteCommon(List<Guid> lstObjectsToValidate)
        {
            using (var connection = new SqlConnection(
                 ConnectionManager.ConfigConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                {
                    connection.Open();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.CommandText = "UnApplySalaryAdjustments";
                    var guids = lstObjectsToValidate.Select(p => p).ToList();
                    DataTable dtGuidList = new DataTable();
                    dtGuidList.Columns.Add("ID", typeof(string));
                    guids.ForEach(p =>
                    {
                        dtGuidList.Rows.Add(p);
                    });
                    SqlParameter param = new SqlParameter("@EmployeeSalaryIncreaseIds", SqlDbType.Structured)
                    {
                        TypeName = "dbo.guidlisttabletype",
                        Value = dtGuidList
                    };
                    command.Parameters.Add(param);
                    //Execute SP de unapply salary adjustment
                    command.ExecuteNonQuery();
                }
            }
        }

        public void AfterCreateUpdate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var companyID = lstObjectsToValidate.FirstOrDefault().company;
            var user = lstObjectsToValidate.FirstOrDefault().user;
            var employeesIds = lstObjectsToValidate.Select(p => p.EmployeeID);

            //Get Employees
            var middlewareManagerEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = middlewareManagerEmployee.FindByExpression(p =>
                p.InstanceID == instanceID &&
                employeesIds.Contains(p.ID), companyID);

            //PeriodTypes of Employees
            var periodTypeIds = employees.Select(p => p.PeriodTypeID);

            //Get Period Details
            var middlewareManagerPD = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetails = middlewareManagerPD.FindByExpression(p =>
                    p.InstanceID == instanceID &&
                    p.PeriodStatus == PeriodStatus.Calculating &&
                    periodTypeIds.Contains(p.Period.PeriodTypeID),
                companyID);

            //Modify directly to calcuting period
            foreach (var periodDetail in periodDetails)
            {
                using (var connection = new SqlConnection(
                   ConnectionManager.ConfigConnectionString))
                {
                    if (connection.State != ConnectionState.Open)
                    {
                        connection.Open();
                    }

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandType = CommandType.StoredProcedure;
                        command.CommandText = "ApplySalaryAdjustments";
                        command.Parameters.AddWithValue("@PeriodDetailId", periodDetail.ID);
                        command.Parameters.AddWithValue("@InstanceId", instanceID);
                        command.Parameters.AddWithValue("@company", companyID);
                        command.Parameters.AddWithValue("@user", user);

                        //Execute SP de autorización
                        command.ExecuteNonQuery();
                    }
                }
            }
        }

        public void AfterCreate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            AfterCreateUpdate(lstObjectsToValidate);
        }

        public void AfterUpdate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            AfterCreateUpdate(lstObjectsToValidate);


        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {

            throw new NotSupportedException();

        }

        public void AfterDelete(List<Guid> lstObjectsToValidate, object parameters)
        {
            List<EmployeeSBCAdjustment> listOfSBC = (List<EmployeeSBCAdjustment>)parameters;

            _middlewareManagerSBCAdjustment.DeleteAsync(listOfSBC.Select(x => x.ID).ToList(), listOfSBC.FirstOrDefault().company).Wait();

        }

        private void ValidateBeforeCreateAndUpdate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            var instanceID = lstObjectsToValidate.FirstOrDefault().InstanceID;
            var companyID = lstObjectsToValidate.FirstOrDefault().company;
            var employeesIds = lstObjectsToValidate.Select(p => p.EmployeeID);

            //Get Employees
            var middlewareManagerEmployee = new MiddlewareManager<Employee>(new BaseRecordManager<Employee>(), new EmployeeValidator());
            var employees = middlewareManagerEmployee.FindByExpression(p => p.InstanceID == instanceID &&
                employeesIds.Contains(p.ID), companyID);

            //PeriodTypes of Employees
            var periodTypeIds = employees.Select(p => p.PeriodTypeID);

            //Get Period Details
            var middlewareManagerPD = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            var periodDetails = middlewareManagerPD.FindByExpression(p =>
                    p.InstanceID == instanceID &&
                    (p.PeriodStatus == PeriodStatus.Open ||
                    p.PeriodStatus == PeriodStatus.Calculating) &&
                    periodTypeIds.Contains(p.Period.PeriodTypeID),
                companyID);

            lstObjectsToValidate.ForEach(increase =>
            {
                var modificationDate = increase.ModificationDate;
                var periodDetail = periodDetails.FirstOrDefault(p => p.InitialDate <= modificationDate
                    && p.FinalDate >= modificationDate);

                var ids = lstObjectsToValidate.Select(p => p.ID);
                //El incremento de salario no debe de coincidir con otro dentro del mismo periodo 
                var employeeSelected = employees.FirstOrDefault(p => p.ID == increase.EmployeeID);
                if (periodDetail != null)
                {
                    //Verificar si hay otro incremento salarial para ese periodo
                    var middlewareManagerEmployeeSalaryIncrease = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>(),
                        this);
                    var employeeSalaryIncreases = middlewareManagerEmployeeSalaryIncrease.FindByExpression(p =>
                            p.InstanceID == instanceID &&
                            !ids.Contains(p.ID) &&
                            p.EmployeeID == employeeSelected.ID &&
                            p.ModificationDate >= periodDetail.InitialDate &&
                            p.ModificationDate <= periodDetail.FinalDate,
                        companyID);

                    if (employeeSalaryIncreases.Any())
                    {
                        throw new CotorraException(4004, "4004",
                            $"El incremento de salario para el colaborador {employeeSelected.FullName} para el periodo del {periodDetail.InitialDate.ToShortDateString()} al {periodDetail.FinalDate.ToShortDateString()} ya existe.", null);
                    }
                }
                //El incremento de salario no debe de caer en un periodo cerrado
                else
                {
                    throw new CotorraException(4005, "4005",
                           $"El incremento de salario para el colaborador {employeeSelected.FullName} para el periodo seleccionado no se puede realizar debido a que el periodo está cerrado.", null);
                }
            });

        }

        public void BeforeCreate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            ValidateBeforeCreateAndUpdate(lstObjectsToValidate);
            //all good
            var validator = new RuleValidator<EmployeeSalaryIncrease>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            throw new NotSupportedException();

        }

        public object BeforeDelete(List<Guid> lstObjectsToValidate, Guid identityWorkID)
        {
            //Desaplicar los ajustes de salario del periodo actual (si aplica)
            BeforeDeleteCommon(lstObjectsToValidate);


            var toDelete = MiddlewareManager.FindByExpressionAsync(x => lstObjectsToValidate.Contains(x.ID) && x.Active, Guid.Empty).Result;

            var sbcToDelete = _middlewareManagerSBCAdjustment.FindByExpression(x => toDelete.Select(y => y.EmployeeSBCAdjustmentID).Contains(x.ID), toDelete.FirstOrDefault().company);

            return sbcToDelete;
        }

        public void BeforeUpdate(List<EmployeeSalaryIncrease> lstObjectsToValidate)
        {
            ValidateBeforeCreateAndUpdate(lstObjectsToValidate);
            BeforeDeleteCommon(lstObjectsToValidate.Select(p => p.ID).ToList());
            //all good
            var validator = new RuleValidator<EmployeeSalaryIncrease>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var sbcAdjustments = lstObjectsToValidate.Select(x => x.EmployeeSBCAdjustment);
            var identityWorkID = lstObjectsToValidate.FirstOrDefault().CompanyID;
            _middlewareManagerSBCAdjustment.UpdateAsync(sbcAdjustments.ToList(), identityWorkID).Wait();

            lstObjectsToValidate.ForEach(item => item.EmployeeSBCAdjustment = null);



        }
    }
}
