using CotorraNode.Common.Base.Schema;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public interface IValidationObjectRule
    { 
        Tuple<bool, List<RuleValidation>> ExecuteValidation(List<Object> ToValidate);

    }
}
