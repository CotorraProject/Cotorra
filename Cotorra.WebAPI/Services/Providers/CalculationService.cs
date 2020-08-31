using AutoMapper;
using Cotorra.Core.Managers.Calculation;
using Cotorra.Schema;
using Cotorra.Schema.Calculation;
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{
    public class CalculationService : ICalculationService
    {
        public async Task<OverdraftDI> CalculateOverdraft(CalculateOverdraftDIParams parameters, IMapper mapper)
        {
            OverdraftDI result = new OverdraftDI();
            OverdraftCalculationDIManager mgr = new OverdraftCalculationDIManager();
            var res = await mgr.CalculateAsync(parameters);
            var over = (res as CalculateOverdraftResult).OverdraftResult;
            mapper.Map(over, result);

            return result;

        }

        public async Task<CalculateResult> CalculateFormula(CalculateOverdraftDIParams parameters)
        {
            OverdraftCalculationDIManager mgr = new OverdraftCalculationDIManager();
            var res = await mgr.CalculateFormula(parameters, parameters.Formula);
            return res;

        }
    }

}
