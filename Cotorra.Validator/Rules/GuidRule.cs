using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;

namespace Cotorra.Core.Validator
{
    public class GuidRule : IValidationRule, IValidationObjectRule
    {
        private string FriendlyFieldName { get; set; }
        private string FieldName { get; set; }
        private int ErrorCode { get; set; }


        public GuidRule(string fieldName, string friendlyFieldName, int erorCode)
        {
            this.FieldName = fieldName;
            this.ErrorCode = erorCode;
            this.FriendlyFieldName = friendlyFieldName;
        }


        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate)
        {
            List<RuleValidation> validations = new List<RuleValidation>();

            for (int i = 0; i < ToValidate.Count; i++)
            {
                var obj = ToValidate[i];
                PropertyInfo fld = obj.GetType().GetProperty(FieldName);
                var value = Guid.Parse((fld.GetValue(obj).ToString()));

                if (value == Guid.Empty)
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value.ToString(),
                    });

                }
            }
            if (validations.Any())
            {
                return Tuple.Create(false, validations);
            }
            return Tuple.Create(true, new List<RuleValidation>());
        }

        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<object> ToValidate)
        {
            List<RuleValidation> validations = new List<RuleValidation>();

            for (int i = 0; i < ToValidate.Count; i++)
            {
                var obj = ToValidate[i];
                PropertyInfo fld = obj.GetType().GetProperty(FieldName);
                var value = Guid.Parse((fld.GetValue(obj).ToString()));

                if (value == Guid.Empty)
                {
                    validations.Add(new RuleValidation()
                    {
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value.ToString(),
                    });

                }
            }
            if (validations.Any())
            {
                return Tuple.Create(false, validations);
            }
            return Tuple.Create(true, new List<RuleValidation>());
        }

       
    }
}
