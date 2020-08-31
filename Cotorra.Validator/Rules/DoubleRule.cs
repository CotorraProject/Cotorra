using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class  DoubleRule: IValidationRule
    {
        public string FriendlyFieldName { get; set; }
        public string FieldName { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public int ErrorCode { get; set; }


        public DoubleRule(string fieldName, string friendlyFieldName,  double min, double max, int erorCode)
        {
            this.Max = max;
            this.Min = min; 
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
                var value = Convert.ToDouble(fld.GetValue(obj));
 

                if (value < Min)
                {

                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser mayor a : " + Min,
                        ErrorCode = ErrorCode,
                        Field = FieldName,
                        ValueSent = value.ToString(),
                    });

                }
                if (value > Max)
                {
                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser menor a : " + Max,
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
