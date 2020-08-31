using Cotorra.Core;
using Cotorra.Schema;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class SettlementProcessClientLocal : ISettlementProcessClient
    {
        public SettlementProcessClientLocal(string authorizationHeader)
        {
        }

        public async Task<ApplySettlementProcessResult> ApplySettlement(ApplySettlementProcessParams parameters)
        {
            SettlementProcessManager mgr = new SettlementProcessManager();
            return await mgr.ApplySettlement(parameters);
        }

        public async Task<List<Overdraft>> Calculate(CalculateSettlementProcessParams parameters)
        {
            SettlementProcessManager mgr = new SettlementProcessManager();
            return await mgr.Calculate(parameters);
        }

        public async Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters)
        {
            SettlementProcessManager mgr = new SettlementProcessManager();
            return await mgr.GenerateSettlementLetter(parameters);
        }
    }
}
