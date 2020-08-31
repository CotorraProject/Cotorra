using CotorraNode.Common.Config;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Linq.Dynamic;

namespace Cotorra.Core.Validator
{
    public class EmployeeSBCAdjustmentValidator : IValidator<EmployeeSBCAdjustment>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<EmployeeSBCAdjustment> MiddlewareManager { get; set; } 

        public EmployeeSBCAdjustmentValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 4002));
            createRules.Add(new GuidRule("EmployeeID", "ID Empleado", 4002));

            createRules.Add(new DuplicateItemRule<EmployeeSBCAdjustment>(new string[] { "EmployeeID", "ModificationDate" }, "Ya existe un movimiento de incremento de salario para este colaborador en la fecha seleccionada.", this, 4003));

          
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
                    command.CommandText = "UnApplySBCAdjustments";
                    var guids = lstObjectsToValidate.Select(p => p).ToList();
                    DataTable dtGuidList = new DataTable();
                    dtGuidList.Columns.Add("ID", typeof(string));
                    guids.ForEach(p =>
                    {
                        dtGuidList.Rows.Add(p);
                    });
                    SqlParameter param = new SqlParameter("@EmployeeSBCAdjustmentsIds", SqlDbType.Structured)
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

        public void AfterCreateUpdate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
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

        public void AfterCreate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
        {
            AfterCreateUpdate(lstObjectsToValidate);
        }

        public void AfterUpdate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
        {
            AfterCreateUpdate(lstObjectsToValidate);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        private void ValidateBeforeCreateAndUpdate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
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
                    var middlewareManagerEmployeeSBCAdjustment = new MiddlewareManager<EmployeeSBCAdjustment>(new BaseRecordManager<EmployeeSBCAdjustment>(), new EmployeeSBCAdjustmentValidator());
                    var employeeSBCAdjustment = middlewareManagerEmployeeSBCAdjustment.FindByExpression(p =>
                            p.InstanceID == instanceID &&
                            !ids.Contains(p.ID) &&
                            p.EmployeeID == employeeSelected.ID &&
                            p.ModificationDate >= periodDetail.InitialDate &&
                            p.ModificationDate <= periodDetail.FinalDate,
                        companyID);

                    if (employeeSBCAdjustment.Any())
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

        public void BeforeCreate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
        {
            ValidateBeforeCreateAndUpdate(lstObjectsToValidate);
            //all good
            var validator = new RuleValidator<EmployeeSBCAdjustment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {

            var employeeSalaryIncreaseMiddlewareManager = new MiddlewareManager<EmployeeSalaryIncrease>(new BaseRecordManager<EmployeeSalaryIncrease>(), new EmployeeSalaryIncreaseValidator());

            if (employeeSalaryIncreaseMiddlewareManager.FindByExpression(p => lstObjectsToValidate.Contains(p.EmployeeSBCAdjustmentID),
                Guid.Empty).Any())
            {
                throw new CotorraException(6901, "6901", "No se puede eliminar la modificación de SBC porque fue creado desde un incremento de salario," +
                    " elimina el aumento de salario relacionado.", null);

            }

            //Desaplicar los ajustes de salario del periodo actual (si aplica)
            BeforeDeleteCommon(lstObjectsToValidate);
        }

        public void BeforeUpdate(List<EmployeeSBCAdjustment> lstObjectsToValidate)
        {
            ValidateBeforeCreateAndUpdate(lstObjectsToValidate);
            BeforeDeleteCommon(lstObjectsToValidate.Select(p => p.ID).ToList());
            //all good
            var validator = new RuleValidator<EmployeeSBCAdjustment>();
            validator.ValidateRules(lstObjectsToValidate, createRules);


        }
    }
}
