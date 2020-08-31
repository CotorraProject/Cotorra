using CotorraNube.CommonApp.Schema;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.FONACOT
{
    public class RetencionFONACOTPeriodoFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        static readonly object _locker = new object();
        #endregion

        #region "Constructor"
        public RetencionFONACOTPeriodoFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
        }

        public RetencionFONACOTPeriodoFunction(FunctionParams functionParams, double x)
        {
            _functionParams = functionParams;
            _x = x;
        }
        #endregion

        public int getParametersNumber()
        {
            return 1;
        }
        public void setParameterValue(int argumentIndex, double argumentValue)
        {
            _x = argumentValue;
        }
        public double calculate()
        {
            //Esta variable obtiene la suma del importe a retener de los créditos activos para el concepto D61 FONACOT en el periodo
            var initialPeriodDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalPeriodDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var fonacotMovements = _functionParams.CalculationBaseResult.FonacotMovements;
            var result = 0.0;

            if (fonacotMovements.Any())
            {               
                foreach (var fonacotMovement in fonacotMovements)
                {
                    //La fecha de inicio del crédito fonacot debe de ser menor a la fecha final del periodo
                    if (fonacotMovement.EmployeeConceptsRelation.InitialCreditDate <= finalPeriodDate)
                    {
                        //Importe fijo
                        /*Tipo retención importe fijo
                        Dado un tipo de retención importe fijo Cuando se calcula Entonces se toma el importe por 
                        periodo y esa es la retención. 
                        Dado que se calcula la retención Cuando el Monto acumulado mas la retención del periodo 
                        actual supera el Monto del crédito Entonces la retención del periodo solo será la diferencia 
                        entre  Monto del crédito - Monto acumulado (antes del periodo actual).*/
                        var periodRetention = fonacotMovement.EmployeeConceptsRelation.OverdraftDetailAmount;
                        if (fonacotMovement.RetentionType == RetentionType.FixedAmount)
                        {
                            if (periodRetention > fonacotMovement.EmployeeConceptsRelation.BalanceCalculated)
                            {
                                periodRetention = fonacotMovement.EmployeeConceptsRelation.BalanceCalculated;
                            }
                        }
                        else if (fonacotMovement.RetentionType == RetentionType.ProportionDaysPayed)
                        {
                            //Valor del Salario (días pagados)
                            var salaryOverdraftDetail = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                                .FirstOrDefault(p =>
                                    p.ConceptPayment.Code == 1 &&
                                    p.ConceptPayment.ConceptType == ConceptType.SalaryPayment);

                            //Valor del séptimo día
                            var seventhDaysOverdraftDetail = _functionParams.CalculationBaseResult.Overdraft.OverdraftDetails
                               .FirstOrDefault(p =>
                                   p.ConceptPayment.Code == 3 &&
                                   p.ConceptPayment.ConceptType == ConceptType.SalaryPayment);
                            var paymentDays = salaryOverdraftDetail.Value + seventhDaysOverdraftDetail.Value;

                            /*Tipo de retención  Proporción a días trabajados
                                Dado que el tipo de retención es Proporción a días pagados Cuando se calcula Entonces 
                            la fórmula es (Retención mensual / 30.4) * días pagados.
                                Días pagados: debe considerar incidencias, de manera simple debe ser homologo a como se 
                            calcula los días del sueldo + los séptimos días + vacaciones
                                Dado que se calcula la retención Cuando el Monto acumulado mas la retención del periodo 
                            actual supera el Monto del crédito Entonces la retención 
                              del periodo solo será la diferencia entre Monto del crédito - Monto acumulado 
                            (antes del periodo actual).*/
                            periodRetention = (periodRetention / 30.4M) * paymentDays;

                            if (periodRetention > fonacotMovement.EmployeeConceptsRelation.BalanceCalculated)
                            {
                                periodRetention = fonacotMovement.EmployeeConceptsRelation.BalanceCalculated;
                            }
                        }

                        var employeeConceptRelationDetail = new EmployeeConceptsRelationDetail()
                        {
                            Active = true,
                            AmountApplied = periodRetention,
                            company = _functionParams.IdentityWorkID,
                            ConceptsRelationPaymentStatus = ConceptsRelationPaymentStatus.Pending,
                            CreationDate = DateTime.Now,
                            CompanyID = _functionParams.IdentityWorkID,
                            DeleteDate = null,
                            Description = "",
                            EmployeeConceptsRelationID = fonacotMovement.EmployeeConceptsRelationID,
                            ID = Guid.NewGuid(),
                            IdentityID = _functionParams.InstanceID,
                            InstanceID = _functionParams.InstanceID,
                            Name = "",
                            OverdraftID = _functionParams.CalculationBaseResult.Overdraft.ID,
                            PaymentDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate,
                            StatusID = 1,
                            Timestamp = DateTime.Now,
                            ValueApplied = 0,
                            IsAmountAppliedCapturedByUser = false
                        };

                        lock (_locker)
                        {
                            if (!_functionParams.EmployeeConceptsRelationDetails.Any(p => p.EmployeeConceptsRelationID == fonacotMovement.EmployeeConceptsRelationID &&
                                p.OverdraftID == _functionParams.CalculationBaseResult.Overdraft.ID))
                            {
                                _functionParams.EmployeeConceptsRelationDetails.Add(employeeConceptRelationDetail);
                            }
                        }

                        result += Convert.ToDouble(periodRetention);
                    }
                }
            }

            return result;
        }

        public FunctionExtension clone()
        {
            return new RetencionFONACOTPeriodoFunction(_functionParams, _x);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}