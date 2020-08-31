using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Cotorra.Core.Validator
{
    public class SimpleStringRule : IValidationRule, IValidationObjectRule
    {
        public bool Obligatory { get; set; }
        public string FriendlyFieldName { get; set; }
        public string FieldName { get; set; }
        public int Min { get; set; }
        public int Max { get; set; }
        public int ErrorCode { get; set; }


        public SimpleStringRule(string fieldName, string friendlyFieldName, bool obligatory, int min, int max, int errorCode)
        {
            this.Max = max;
            this.Min = min;
            this.Obligatory = obligatory;
            this.FieldName = fieldName;
            this.ErrorCode = errorCode;
            this.FriendlyFieldName = friendlyFieldName;
        }

        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate)
        {
            List<RuleValidation> validations = new List<RuleValidation>();

            for (int i = 0; i < ToValidate.Count; i++)
            {
                var obj = ToValidate[i];
                PropertyInfo fld = obj.GetType()?.GetProperty(FieldName);
                if (null == fld)
                {
                    throw new CotorraException(1100, "1100", $"La propiedad {FieldName} definida en la validación no existe.", null);
                }
                var value = fld.GetValue(obj)?.ToString();
                if (Obligatory && string.IsNullOrEmpty(value))
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
                    });
                }

                if (!string.IsNullOrEmpty(value) && value.Length < Min)
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser mayor a : " + Min + " caracteres",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
                    });

                }
                if (null != value && value.Length > Max)
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser menor a : " + Max + " caracteres",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
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
                var value = fld.GetValue(obj).ToString();
                if (Obligatory && string.IsNullOrEmpty(value))
                {
                    validations.Add(new RuleValidation()
                    {

                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " es obligatorio",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
                    });
                }

                if (!string.IsNullOrEmpty(value) && value.Length < Min)
                {
                    validations.Add(new RuleValidation()
                    {

                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser mayor a : " + Min + " caracteres",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
                    });

                }
                if (!string.IsNullOrEmpty(value) && value.Length > Max)
                {
                    validations.Add(new RuleValidation()
                    {
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser menor a : " + Max + " caracteres",
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value,
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
