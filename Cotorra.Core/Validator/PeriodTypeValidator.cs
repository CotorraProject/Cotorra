using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PeriodTypeValidator : IValidator<PeriodType>
    {
        static List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<PeriodType> MiddlewareManager { get; set; }
        static PeriodTypeValidator()
        {
            createRules.Add(new SimpleStringRule("Name", "nombre", true, 1, 50, 9001));
            createRules.Add(new IntRule("PeriodTotalDays", "Total de días del periodo", 0, 365, 9002));
            createRules.Add(new DecimalRule("PaymentDays", "Días de pago", 1, 365, 9003));
            createRules.Add(new SimpleStringRule("SeventhDayPosition", "Posición de septimo día", false, 0, 200, 9004));
            createRules.Add(new IntRule("PaymentDayPosition", "Posición del día de pago", 0, 100, 9005));
            createRules.Add(new IntRule("PaymentPeriodicity", "Periodicidad", 0, 100, 9006));
        }

        public void AfterCreate(List<PeriodType> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void AfterUpdate(List<PeriodType> lstObjectsToValidate)
        {
            //all good
            var periodMiddlewareManager = new MiddlewareManager<Period>(new BaseRecordManager<Period>(),
                new PeriodValidator());

            var company = lstObjectsToValidate.FirstOrDefault().company;
            var periodTypeIDs = lstObjectsToValidate.Select(p => p.ID);

            //Obtener los periodos y sus detalles
            var periods = periodMiddlewareManager.FindByExpression(p => periodTypeIDs.Contains(p.PeriodTypeID), 
                company, new string[] { "PeriodDetails" });

            foreach (var period in periods)
            {
                var periodTypeModified = lstObjectsToValidate.FirstOrDefault(p => p.ID == period.PeriodTypeID);
                //Por cada periodo se update los días de pago y sus fortnight
                period.FortnightPaymentDays = periodTypeModified.FortnightPaymentDays;
                period.PaymentDays = periodTypeModified.PaymentDays;
                foreach (var periodDetail in period.PeriodDetails)
                {
                    if (periodDetail.PeriodStatus != PeriodStatus.Authorized &&
                        periodDetail.PeriodStatus != PeriodStatus.Stamped)
                    {
                        //Si es por los días de pago
                        if (periodTypeModified.FortnightPaymentDays == AdjustmentPay_16Days_Febrary.PayPaymentDays)
                        {
                            periodDetail.PaymentDays = periodTypeModified.PaymentDays;
                        }
                        //Si es por los días calendario
                        else
                        {
                            periodDetail.PaymentDays = Convert.ToDecimal((periodDetail.FinalDate - periodDetail.InitialDate).TotalDays + 1);
                        }
                    }
                }
            }
            //al finalizar se actualizan
            periodMiddlewareManager.Update(periods, company);
        }

        public void BeforeCreate(List<PeriodType> lstObjectsToValidate)
        {
            var validator = new RuleValidator<PeriodType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            //all good
        }

        public void BeforeUpdate(List<PeriodType> lstObjectsToValidate)
        {
            var validator = new RuleValidator<PeriodType>();
            validator.ValidateRules(lstObjectsToValidate, createRules);
        }


    }
}
