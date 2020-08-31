using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class SettlementProcessClient : ISettlementProcessClient
    {
        private readonly ISettlementProcessClient _client;

        public SettlementProcessClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<ISettlementProcessClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task<ApplySettlementProcessResult> ApplySettlement(ApplySettlementProcessParams parameters)
        {
            return _client.ApplySettlement(parameters);
        }

        public Task<List<Overdraft>> Calculate(CalculateSettlementProcessParams parameters)
        {
            return _client.Calculate(parameters);
        }

        public Task<string> GenerateSettlementLetter(GenerateSettlementLetterParams parameters)
        {
            return _client.GenerateSettlementLetter(parameters);
        }
    }
}

