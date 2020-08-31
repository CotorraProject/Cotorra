using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class  DoubleValueInOptionsRule: IValidationRule
    {
        public string FriendlyFieldName { get; set; }
        public string FieldName { get; set; }
        public List<double> Options { get; set; }
        public double Max { get; set; }
        public int ErrorCode { get; set; }
       

        public DoubleValueInOptionsRule(string fieldName, string friendlyFieldName,  List<double> options, int erorCode)
        {

            this.FieldName = fieldName;
            this.ErrorCode = erorCode;
            this.FriendlyFieldName = friendlyFieldName;
            this.Options = options;
        }


        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate)
        {
            List<RuleValidation> validations = new List<RuleValidation>();

            for (int i = 0; i < ToValidate.Count; i++)
            {
                var obj = ToValidate[i];
                PropertyInfo fld = obj.GetType().GetProperty(FieldName);
                var value = Convert.ToDouble(fld.GetValue(obj));
 

                if (!Options.Contains(value))
                {

                    validations.Add(new RuleValidation()
                    {
                        ID = obj.ID,
                        IsValidValue = false,
                        Message = "El dato " + FriendlyFieldName + " debe ser alguno de los siguientes valores : " + FormatValues(),
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

        private string FormatValues( )
        {
            var res = "";
            Options.ForEach(item =>
            {
                res += item + " ";
            });
            return res;
        }

       
    }


}
