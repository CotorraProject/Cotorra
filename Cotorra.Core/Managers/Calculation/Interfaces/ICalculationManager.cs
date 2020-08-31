using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public interface ICalculationManager
    {
        Task<ICalculateResult> CalculateAsync(ICalculateParams calculateParams);

        CalculateResult CalculateFormula(string formula);
    }
}
