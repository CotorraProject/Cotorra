using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class EmployeeConceptsRelationDetailClient : IEmployeeConceptsRelationDetailClient
    {
        private readonly IEmployeeConceptsRelationDetailClient _client;
        public EmployeeConceptsRelationDetailClient(string authorizationHeader, ClientConfiguration.ClientAdapter clientadapter = ClientConfiguration.ClientAdapter.Proxy)
        {
            _client = ClientAdapterFactory2.GetInstance<IEmployeeConceptsRelationDetailClient>(this.GetType(), authorizationHeader, clientadapter: clientadapter);
        }

        public Task UpdateAmountAsync(Guid OverdraftDetailID, decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied,
             Guid instanceID, Guid identityWorkID)
        {
            return _client.UpdateAmountAsync(OverdraftDetailID, Amount, ConceptRelationsAmountApplied,
                   instanceID, identityWorkID);
        }
    }
}

