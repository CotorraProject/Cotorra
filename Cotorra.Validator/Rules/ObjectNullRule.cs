using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;

namespace Cotorra.Core.Validator
{
    public class ObjectNullRule : IValidationRule, IValidationObjectRule
    {
        private string FriendlyFieldName { get; set; }
        private string FieldName { get; set; }
        private int ErrorCode { get; set; }


        public ObjectNullRule(string fieldName, string friendlyFieldName, int erorCode)
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
                var value = fld.GetValue(obj);

                if (value == null)
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = "Null",

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
                var value = fld.GetValue(obj);

                if (value == null)
                {
                    validations.Add(new RuleValidation()
                    {
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = "Null",
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
