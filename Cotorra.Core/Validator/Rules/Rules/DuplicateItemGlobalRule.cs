using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;


namespace Cotorra.Core.Validator
{
    public class DuplicateItemGlobalRule<T> : IValidationRule where T : BaseEntity, ICompanyData
    {
        public string FriendlyFieldName { get; set; }
        public string[] FieldName { get; set; }
        public int ErrorCode { get; set; }

        private IValidator<T> _validator;

        public DuplicateItemGlobalRule(string[] fieldName, string friendlyFieldName, IValidator<T> validator, int erorCode)
        {
            this.FieldName = fieldName;
            this.ErrorCode = erorCode;
            this.FriendlyFieldName = friendlyFieldName;
            this._validator = validator;
        }

        public Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate)
        {
            try
            {
                List<RuleValidation> validations = new List<RuleValidation>();
                var jstr = new StringBuilder();
                var companyData = ToValidate.FirstOrDefault() as ICompanyData;
                var ids = ToValidate.Select(p => p.ID).ToList();
                Dictionary<string, string> dictDuplicates = new Dictionary<string, string>();

                //duplicates in updates :)
                jstr.Append($"(");
                for (int i = 0; i < ids.Count; i++)
                {
                    jstr.Append($"!ID.Equals(Guid.Parse(\"{ids[i]}\"))");

                    if (i < ids.Count - 1)
                    {
                        jstr.Append($" and ");
                    }
                }
                jstr.Append($") and (");

                //normal query
                for (int i = 0; i < ToValidate.Count; i++)
                {
                    var obj = ToValidate[i];
                    StringBuilder key = new StringBuilder();
                    jstr.Append("(");
                    for (int j = 0; j < FieldName.Count(); j++)
                    {
                        PropertyInfo fld = obj.GetType().GetProperty(FieldName[j]);
                        var value = fld.GetValue(obj);
                        key.Append(FieldName[j] + value);

                        if (value == null)
                        {
                            validations.Add(new RuleValidation()
                            {
                                ID = Guid.Empty,
                                IsValidValue = false,
                                Message = $"El campo {FieldName[j]} no fue proporcionado y es obligatorio.",
                                ErrorCode = ErrorCode,
                                Field = FieldName[0],
                                ValueSent = $"Campo compuesto: {key.ToString()}"
                            });
                            break;
                        }

                        if (fld.PropertyType.FullName.Contains("Int"))
                        {
                            jstr.Append($"{fld.Name}.Equals(Int32.Parse(\"{value.ToString()}\"))");
                        }
                        else if (fld.PropertyType.FullName.Contains("System.Nullable`1[[System.Guid"))
                        {
                            jstr.Append($"{fld.Name}.Value.Equals(Guid.Parse(\"{value.ToString()}\"))");
                        }
                        else if (fld.PropertyType.FullName.Contains("Guid"))
                        {
                            jstr.Append($"{fld.Name}.Equals(Guid.Parse(\"{value.ToString()}\"))");
                        }
                        else if (fld.PropertyType.FullName.Contains("DateTime"))
                        {
                            jstr.Append($"{fld.Name}.Equals(DateTime.Parse(\"{value.ToString()}\"))");
                        }
                        else
                        {
                            jstr.Append($"{fld.Name} == \"{value.ToString()}\"");
                        }

                        if (j < (FieldName.Count() - 1))
                        {
                            jstr.Append(" and ");
                        }
                    }
                    jstr.Append(")");

                    if (!dictDuplicates.TryAdd(key.ToString(), ""))
                    {
                        validations.Add(new RuleValidation()
                        {
                            ID = Guid.Empty,
                            IsValidValue = false,
                            Message = $"Existe 1 o más registros duplicados en la lista que se quieren insertar con el campo: {FriendlyFieldName}.",
                            ErrorCode = ErrorCode,
                            Field = FieldName[0],
                            ValueSent = $"Campo compuesto: {key.ToString()}"
                        });
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

                if (!validations.Any())
                {
                    if (null == _validator.MiddlewareManager)
                    {
                        _validator.MiddlewareManager = new MiddlewareManager<T>(new BaseRecordManager<T>(), _validator);
                    }

                    var query = jstr.ToString();
                    var result = _validator.MiddlewareManager.Find(query, companyData.company, null);

                    if (result.Any())
                    {
                        validations.Add(new RuleValidation()
                        {
                            ID = Guid.Empty,
                            IsValidValue = false,
                            Message = $"Existe 1 o más registros que ya existen con el campo: {FriendlyFieldName}.",
                            ErrorCode = ErrorCode,
                            Field = FieldName[0],
                            ValueSent = $"ID: {String.Join(String.Empty, result.Select(p => p.ID.ToString()))}"
                        });
                    }
                }

                if (validations.Any())
                {
                    if (!validations.Any(p => p.Message.Contains("no fue proporcionado y es obligatorio")))
                    {
                        return Tuple.Create(false, validations);
                    }
                    else
                    {
                        return Tuple.Create(true, new List<RuleValidation>());
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Ocurrió un error al validar duplicados.", ex);
            }
            return Tuple.Create(true, new List<RuleValidation>());
        }


    }
}
