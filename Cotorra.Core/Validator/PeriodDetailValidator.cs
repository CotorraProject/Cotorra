using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class PeriodDetailValidator : IValidator<PeriodDetail>
    {
        private List<IValidationRule> createRules = new List<IValidationRule>();
        public IMiddlewareManager<PeriodDetail> MiddlewareManager { get; set; }
        public PeriodDetailValidator()
        {
            createRules.Add(new GuidRule("InstanceID", "Instancia", 8001));
            createRules.Add(new SimpleStringRule("Name", "Nombre", true, 1, 40, 8002));
            createRules.Add(new GuidRule("PeriodID", "Periodo", 8003));
            createRules.Add(new IntRule("PaymentDays", "Días de pago", 1, 32, 8004));
        }

        public void AfterCreate(List<PeriodDetail> lstObjectsToValidate)
        {
        }

        public void AfterDelete(List<Guid> lstObjectsToValidate)
        {
        }

        public void AfterUpdate(List<PeriodDetail> lstObjectsToValidate)
        {
        }

        public void BeforeCreate(List<PeriodDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<PeriodDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), this);

            foreach (var entity in lstObjectsToValidate)
            {
                var result = middlewareManager.FindByExpression(p =>
                    p.InstanceID == entity.InstanceID &&
                    p.ID != entity.ID &&
                    p.PeriodID == entity.PeriodID &&
                    ((p.Number == (entity.Number - 1) && entity.InitialDate <= p.FinalDate) ||
                    (p.Number == (entity.Number + 1) && entity.FinalDate >= p.InitialDate)) &&
                    p.Active, Guid.Empty, null);

                if (result.Any())
                {
                    throw new CotorraException(8008, "8008", "No se puede actualizar este periodo, debido a que la fecha se traslapa con otros periodos.", new Exception(""));
                }
            }
        }

        public void BeforeDelete(List<Guid> lstObjectsToValidate)
        {
            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), this);
            var entities = middlewareManager.FindByExpression(p => lstObjectsToValidate.Contains(p.ID), Guid.Empty, null);

            foreach (var entity in entities)
            {
                var result = middlewareManager.FindByExpression(p => p.InstanceID == entity.InstanceID && p.Number > entity.Number && p.Active, Guid.Empty, null);

                result = result.Where(p => !lstObjectsToValidate.Contains(p.ID)).ToList();

                if (result.Any())
                {
                    throw new CotorraException(8007, "8007", "No se puede eliminar este periodo, debido a que tiene periodos futuros abiertos.", new Exception(""));
                }
            }
        }

        public void BeforeUpdate(List<PeriodDetail> lstObjectsToValidate)
        {
            //all good
            var validator = new RuleValidator<PeriodDetail>();
            validator.ValidateRules(lstObjectsToValidate, createRules);

            var middlewareManager = new MiddlewareManager<PeriodDetail>(new BaseRecordManager<PeriodDetail>(), this);
            foreach (var entity in lstObjectsToValidate)
            {
                var result = middlewareManager.FindByExpression(p =>
                     p.InstanceID == entity.InstanceID &&
                     p.ID != entity.ID &&
                     p.PeriodID == entity.PeriodID &&
                     ((p.Number == (entity.Number - 1) && entity.InitialDate <= p.FinalDate) ||
                     (p.Number == (entity.Number + 1) && entity.FinalDate >= p.InitialDate)) &&
                     p.Active, Guid.Empty, null);

                if (result.Any())
                {
                    throw new CotorraException(8008, "8008", "No se puede actualizar este periodo, debido a que la fecha se traslapa con otros periodos.", new Exception(""));
                }
            }
        }
    }
}
