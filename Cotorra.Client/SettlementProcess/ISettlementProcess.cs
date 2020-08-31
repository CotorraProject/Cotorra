using Cotorra.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface ISettlementProcessClient
    {
        Task<List<Overdraft>> Calculate(CalculateSettlementProcessParams parameters);
        Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters);
        Task<ApplySettlementProcessResult> ApplySettlement(ApplySettlementProcessParams parameters);

    }
}
