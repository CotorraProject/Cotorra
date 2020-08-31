using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class ForeignKeyRule<T> : IValidationRule where T : BaseEntity, ICompanyData
    {
        public string FriendlyFieldName { get; set; }
        public string[] FieldName { get; set; }
        public int ErrorCode { get; set; }
        private IValidator<T> _validator;

        public ForeignKeyRule(string[] fieldName, string friendlyFieldName, IValidator<T> validator, int erorCode)
        {
            this.FieldName = fieldName;
            this.ErrorCode = erorCode;
            this.FriendlyFieldName = friendlyFieldName;
            this._validator = validator;
        }

        public ForeignKeyRule(string friendlyFieldName, IValidator<T> validator, int erorCode)
        {
            this.FieldName = new string[] { "ID" };
            this.ErrorCode = erorCode;
            this.FriendlyFieldName = friendlyFieldName;
            this._validator = validator;
        }

        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate)
        {
            List<RuleValidation> validations = new List<RuleValidation>();
            var jstr = new StringBuilder();
            var companyData = ToValidate.FirstOrDefault() as ICompanyData;
            PropertyInfo fldInstance = companyData.GetType().GetProperty("InstanceID");
            var valueInstance = fldInstance.GetValue(companyData);
            Dictionary<string, string> dictDuplicates = new Dictionary<string, string>();

            jstr.Append($"InstanceID.Equals(Guid.Parse(\"{valueInstance.ToString()}\")) and (");
            for (int i = 0; i < ToValidate.Count; i++)
            {
                var obj = ToValidate[i];

                for (int j = 0; j < FieldName.Count(); j++)
                {
                    PropertyInfo fld = obj.GetType().GetProperty(FieldName[j]);
                    var value = fld.GetValue(obj);

                    jstr.Append($"ID.Equals(Guid.Parse(\"{value.ToString()}\"))");

                    if (j < (FieldName.Count() - 1))
                    {
                        jstr = jstr.Append(" and ");
                    }
                }

                if (i == (ToValidate.Count() - 1))
                {
                    jstr = jstr.Append(" ) ");
                }
                else
                {
                    jstr = jstr.Append(" or ");
                }

            }

            if (null == _validator.MiddlewareManager)
            {
                _validator.MiddlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), _validator);
            }

            var result = _validator.MiddlewareManager.Find(jstr.ToString(), companyData.company, null);

            if (!result.Any())
            {
                validations.Add(new RuleValidation()
                {
                    ID = Guid.Empty,
                    IsValidValue = false,
                    Message = $"No existe la relación con: {FriendlyFieldName}.",
                    ErrorCode = ErrorCode,
                    Field = FieldName[0],
                    ValueSent = $"ID: {String.Join(String.Empty, result.Select(p => p.ID.ToString()))}"
                });
            }

            if (validations.Any())
            {
                return Tuple.Create(false, validations);
            }
            return Tuple.Create(true, new List<RuleValidation>());
        }
    }
}
