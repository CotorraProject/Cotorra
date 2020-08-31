using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public interface IValidationRule
    {
        Tuple<bool, List<RuleValidation>> ExecuteValidation(List<BaseEntity> ToValidate);
      

    }
}
