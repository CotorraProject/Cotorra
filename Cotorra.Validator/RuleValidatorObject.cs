using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Cotorra.Core.Validator

{
    public class RuleValidatorObject<T>
    {
        public void ValidateRules(List<T> lstObjectsToValidate, List<IValidationObjectRule> rules)
        {
            List<RuleValidationReg> validations = new List<RuleValidationReg>();
            List<RuleValidation> res = new List<RuleValidation>();
            for (int i = 0; i < rules.Count; i++)
            {
                var validationsRes = rules[i].ExecuteValidation(lstObjectsToValidate.Cast<Object>().ToList());
                if (validationsRes.Item1 == false)
                {
                    res.AddRange(validationsRes.Item2);
                }
            }

            if (res.Any())
            {
                IEnumerable<IGrouping<Guid, RuleValidation>> query =
                        res.GroupBy(val => val.ID, val => val);

                foreach (IGrouping<Guid, RuleValidation> petGroup in query)
                {
                    validations.Add(new RuleValidationReg()
                    {
                        ID = petGroup.Key,
                        Validations = petGroup.ToList()
                    });
                }

                throw new CotorraException(res.FirstOrDefault().ErrorCode, "", res.FirstOrDefault().Message, null, validations);
            }
        }
    }
}
