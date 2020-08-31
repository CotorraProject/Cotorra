using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PeriodValidator : IValidator<Period>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();

        public IMiddlewareManager<Period> MiddlewareManager { get; set; }
        public PeriodValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 8001));
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 40, 8002));
            createRules.Add(new GuidRule("PeriodTypeID", "Tipo de periodo", 8003));
            createRules.Add(new IntRule("FiscalYear", "Ejercicio", 2000, 3000, 8004));
            createRules.Add(new IntRule("PeriodTotalDays", "Total de días del periodo", 0, 365, 8005));
            createRules.Add(new DecimalRule("PaymentDays", "Días de pago", 1, 365, 8006));
            createRules.Add(new SimpleStringRule("SeventhDayPosition", "Posición de septimo día", false, 0, 200, 8007));
            createRules.Add(new IntRule("PaymentDayPosition", "Posición del día de pago", 0, 100, 8008));
            createRules.Add(new IntRule("PaymentPeriodicity", "Periodicidad", 0, 100, 8009));
            createRules.Add(new DuplicateItemRule<Period>(new string[] { "FiscalYear", "PeriodTypeID" }, "Periodo / Ejercicio duplicado", this, 8010));
        }

        private List<PeriodDetail> CreatePeriodDetailsByPeriodType(Period period, Guid company, Guid instance, Guid user)
        {
            var lstPeriodDetail = new List<PeriodDetail>();

            var payrollCompanyConfigManager = new MiddlewareManager<PayrollCompanyConfiguration>(new BaseRecordManager<PayrollCompanyConfiguration>(),
                new PayrollCompanyConfigurationValidator());
            var payrollCompanyConfig = payrollCompanyConfigManager.FindByExpression(p => p.company == company && p.InstanceID == instance, company);

            if (period.PaymentPeriodicity == PaymentPeriodicity.Biweekly
                || period.PaymentPeriodicity == PaymentPeriodicity.Monthly
                || period.PaymentPeriodicity == PaymentPeriodicity.Weekly)
            {

                var currentPeriod = 1;

                if (payrollCompanyConfig.Any())
                {
                    currentPeriod = payrollCompanyConfig.FirstOrDefault().CurrentPeriod;
                }

                if (period.PaymentPeriodicity == PaymentPeriodicity.Weekly)
                {
                    PeriodGeneratorNoMonthCalendarFixed weeklyGenerator = new PeriodGeneratorNoMonthCalendarFixed();
                    lstPeriodDetail.AddRange(weeklyGenerator.GeneratePeriods(period, currentPeriod, company, instance, user));
                }
                else if (period.PaymentPeriodicity == PaymentPeriodicity.Biweekly
                        || period.PaymentPeriodicity == PaymentPeriodicity.Monthly)
                {
                    PeriodGeneratorMonthCalendarFixed periodGenerator = new PeriodGeneratorMonthCalendarFixed();
                    lstPeriodDetail.AddRange(periodGenerator.GeneratePeriods(period, currentPeriod, company, instance, user));
                }
            }
            
            return lstPeriodDetail;
        }

        public void AfterCreate(List<Period> lstObjectsToValidate)
        {
            var lstPeriodDetails = new List<PeriodDetail>();
            var company = lstObjectsToValidate.FirstOrDefault().company;
            var user = lstObjectsToValidate.FirstOrDefault().user;
            var instance = lstObjectsToValidate.FirstOrDefault().InstanceID;

            lstObjectsToValidate.ForEach(p =>
            {
                if (!p.ExtraordinaryPeriod)
                {
                    lstPeriodDetails.AddRange(CreatePeriodDetailsByPeriodType(p, company, instance, user));
                }
            });

            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), new PeriodDetailValidator());
            middlewareManager.Create(lstPeriodDetails, company);
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<Period> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<Period> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Period>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void BeforeUpdate(List<Period> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<Period>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }
    }
}
