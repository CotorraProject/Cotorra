using AutoMapper; 
using Cotorra.Schema; 
using System.Threading.Tasks;

namespace Cotorra.WebAPI
{

    public interface ICalculationService
    {
        Task<OverdraftDI> CalculateOverdraft(CalculateOverdraftDIParams parameters, IMapper mapper);

        Task<CalculateResult> CalculateFormula(CalculateOverdraftDIParams parameters);

    }
}
