using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.Calculation
{
    public class CalculationGenericManager : CalculationBase, ICalculationManager
    {
        /// <summary>
        /// Calculation generic
        /// </summary>
        /// <param name="calculateParams"></param>
        /// <returns></returns>
        public async Task<ICalculateResult> CalculateAsync(ICalculateParams calculateParams)
        {
            //result instance
            var calculateGenericResult = new CalculateGenericResult();
            var genericParams = calculateParams as CalculateGenericParams;

            var calculateOverdraftParams = new CalculateOverdraftParams() { 
                DeleteAccumulates = true,
                SaveOverdraft = false,
                IdentityWorkID = genericParams.IdentityWorkID,
                InstanceID = genericParams.InstanceID,
                OverdraftID = genericParams.OverdraftID.Value,
                ResetCalculation = false,
            };

            var overdraftCalculationManager = new OverdraftCalculationManager();
            await overdraftCalculationManager.CalculateAsync(calculateOverdraftParams);
            var calculateResult = overdraftCalculationManager.CalculateFormula(genericParams.Formula);

            calculateGenericResult.Result = calculateResult.Result;
            calculateGenericResult.ResultText = calculateResult.ResultText;
            calculateGenericResult.CalculateArguments = calculateResult.CalculateArguments;

            return calculateGenericResult;
        }

        public CalculateResult CalculateFormula(string formula)
        {
            return base.Calculate(formula);
        }
    }
}
