using CotorraNode.Common.Library.Private;
using MoreLinq;
using Cotorra.Core.Managers.Calculation.Functions.Catalogs;
using Cotorra.Schema;
using org.mariuszgromada.math.mxparser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation.Functions.Incidents
{
    public class PeriodEmployeeSalary
    {
        public DateTime ApplicationDate { get; set; }
        public decimal DiarySalary { get; set; }
    }
    public class HEExempt
    {
        public DateTime Day { get; set; }

        public int PaymentTimes { get; set; }

        public double HEExempts { get; set; }
    }

    public class DFT_topesXsemanaFunction : FunctionExtension
    {
        #region "Attributes"
        private FunctionParams _functionParams;
        private double _x;
        #endregion

        #region "Constructor"
        public DFT_topesXsemanaFunction(FunctionParams functionParams)
        {
            _functionParams = functionParams;
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
        private List<HEExempt> GetHEExempts()
        {
            const int MAX_HOURS_PER_DAY = 3;
            const int MAX_DAYS_PER_WEEK = 3;

            List<HEExempt> hEExempts = new List<HEExempt>();

            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            //Sacar los incidentes de horas extras
            var incidentHE1 = _functionParams.CalculationBaseResult.Incidents.Where(p =>
                    p.IncidentType.TypeOfIncident == TypeOfIncident.Hours &&
                    (p.IncidentType.Code == "DFT "));

            //si hay Incidents de tipo horas extras
            if (incidentHE1.Any())
            {
                var initialTempDate = initialDate;
                var finalTempDate = initialTempDate.AddDays(7);

                //Fix - Solamente considerar el maximo de horas por dia de cada día
                incidentHE1.ForEach(p =>
                {
                    if (p.Value > MAX_HOURS_PER_DAY)
                    {
                        p.Value = MAX_HOURS_PER_DAY;
                    }
                });

                while (finalTempDate >= initialDate && finalTempDate <= finalDate &&
                       initialTempDate >= initialDate && initialTempDate <= finalDate)
                {
                    //Obtiene los incidentes de la semana con la condición de _x (maximo numero de horas por semana)
                    //Solamente considerar 3 de 5
                    var incidentsFirstRule = incidentHE1.Where(p =>
                        p.Date >= initialTempDate &&
                        p.Date <= finalTempDate);

                    if (incidentsFirstRule.Any())
                    {
                        incidentsFirstRule = incidentsFirstRule.OrderBy(p => p.Date);
                        //Ordena por valor de mayor a menor y toma los _y (maximo numero de dias por semana)
                        var incidentsPerWeek = incidentsFirstRule.Take(MAX_DAYS_PER_WEEK);
                        incidentsPerWeek.ForEach(incident => {
                            int paymentType = 0;

                            if (incident.IncidentType.Code == "DFT ")
                            {
                                paymentType = 2;
                            }

                            hEExempts.Add(new HEExempt() { Day = incident.Date, HEExempts = Convert.ToDouble(incident.Value), PaymentTimes = paymentType }); 
                        });
                        
                    }

                    initialTempDate = initialTempDate.AddDays(7 + 1);

                    //regla de poner los dias restantes del periodo                        
                    finalTempDate = finalTempDate.AddDays(7);
                    if (finalTempDate > finalDate)
                    {
                        finalTempDate = finalDate;
                    }
                }
            }

            return hEExempts;
        }

        public double He_TopesXSemana(List<HEExempt> horasExentasPorDia,
           double minimunSalary,
           List<PeriodEmployeeSalary> salariosDiariosPeriodo,
           double horasTurnoEmpleado)
        {
            short DIAS_POR_SEMANA = 7;
            short EXENTA_50_PORCIENTO = 2;
            short EXENTA_100_PORCIENTO = 1;
            short diasTranscurridos = 0;
            double exentoPeriodo = 0;
            double exentoPorSemana = 0;
            double sueldoPorHora;
            double sueldoDiario;
            short factorGanaMasDelMinimo = 1;
            double tope5Umas = 0;
            double salarioMinimo = 0;
            DateTime diaPeriodo = DateTime.Today;

            foreach (var diaHorasExentas in horasExentasPorDia)
            {
                diasTranscurridos++;
                diaPeriodo = diaHorasExentas.Day;

                if (diaHorasExentas.HEExempts > 0)
                {
                    sueldoDiario = Convert.ToDouble(salariosDiariosPeriodo.Where(x => x.ApplicationDate <= diaPeriodo).OrderByDescending(s => s.ApplicationDate).FirstOrDefault().DiarySalary);
                    salarioMinimo = minimunSalary;

                    factorGanaMasDelMinimo = (sueldoDiario > salarioMinimo) ? EXENTA_50_PORCIENTO : EXENTA_100_PORCIENTO;

                    sueldoPorHora = sueldoDiario / horasTurnoEmpleado;
                    exentoPorSemana += (sueldoPorHora * diaHorasExentas.HEExempts * diaHorasExentas.PaymentTimes) / factorGanaMasDelMinimo;
                }

                if (diasTranscurridos == DIAS_POR_SEMANA)
                {
                    diasTranscurridos = 0;
                    tope5Umas = getValorUMA(diaHorasExentas.Day) * 5;

                    //Por semana
                    var hes = 0.0;
                    exentoPeriodo += ((exentoPorSemana + hes) > tope5Umas) ? (tope5Umas - hes) : exentoPorSemana;

                    exentoPorSemana = 0;
                }
            }

            if ((diasTranscurridos > 0) && (diasTranscurridos < DIAS_POR_SEMANA))
            {
                diasTranscurridos = 0;
                tope5Umas = getValorUMA(diaPeriodo) * 5;

                //Por semana
                var hes = 0.0;
                exentoPeriodo += ((exentoPorSemana + hes) > tope5Umas) ? (tope5Umas - hes) : exentoPorSemana;
            }

            return exentoPeriodo;
        }

        private double getValorUMA(DateTime dateTime)
        {
            double result = 0.0;
            var umas = _functionParams.CalculationBaseResult.UMAs;

            if (umas.Any())
            {
                var umasInPeriod = umas.Where(p => p.ValidityDate <=  dateTime);
                var umaTop = umasInPeriod.MaxBy(p => p.ValidityDate);
                result = Convert.ToDouble(umaTop.FirstOrDefault().Value);
            }

            return result;
        }

        public double calculate()
        {
            var result = 0.0;

            //Horas por turno del empleado
            var workshiftHours = new HorasPorTurnoFunction(_functionParams).calculate();

            //Salario Minimo
            var minimunSalary = 0.0;
            if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneA)
            {
                minimunSalary = new SalariosMinimos_ZonaA(_functionParams).calculate();
            }
            else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneB)
            {
                minimunSalary = new SalariosMinimos_ZonaB(_functionParams).calculate();
            }
            else if (_functionParams.CalculationBaseResult.Overdraft.Employee.SalaryZone == SalaryZone.ZoneC)
            {
                minimunSalary = new SalariosMinimos_ZonaC(_functionParams).calculate();
            }

            //Period
            var periodEmployeeSalaries = new List<PeriodEmployeeSalary>();
            var initialDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.InitialDate;
            var finalDate = _functionParams.CalculationBaseResult.Overdraft.PeriodDetail.FinalDate;

            //Salarios diarios
            var historicEmployeeSalaryAdjustments = _functionParams.CalculationBaseResult.Overdraft.Employee.HistoricEmployeeSalaryAdjustments.
               Where(p => p.ModificationDate >= initialDate && p.ModificationDate <= finalDate);

            if (historicEmployeeSalaryAdjustments.Any())
            {
                historicEmployeeSalaryAdjustments.ForEach(historic =>
                {
                    periodEmployeeSalaries.Add(new PeriodEmployeeSalary() { ApplicationDate = historic.ModificationDate, DiarySalary = historic.DailySalary });
                });
            }
            else
            {
                var actualDailySalary = new SalDiarioVigenteFunction(_functionParams).calculate();
                periodEmployeeSalaries.Add(new PeriodEmployeeSalary() { ApplicationDate = new DateTime(1900, 1, 1), DiarySalary = Convert.ToDecimal(actualDailySalary) });
            }

            //HE Exempts 
            var heExempts = GetHEExempts();

            result = He_TopesXSemana(heExempts, minimunSalary, periodEmployeeSalaries, workshiftHours);

            return result;
        }
        public FunctionExtension clone()
        {
            return new DFT_topesXsemanaFunction(_functionParams);
        }

        public string getParameterName(int parameterIndex)
        {
            return "FunctionParams";
        }
    }
}