using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Managers.Calculation
{
    public interface IFormulaManager
    {
        FunctionParams FunctionParamsProperty { get; set; }

        CalculateResult Calculate(string formula, bool includeDetail = false);
    }
}
