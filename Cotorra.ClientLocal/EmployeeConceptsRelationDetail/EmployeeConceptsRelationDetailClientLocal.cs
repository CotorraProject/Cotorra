using Cotorra.Core;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class EmployeeConceptsRelationDetailClientLocal : IEmployeeConceptsRelationDetailClient
    {
        public EmployeeConceptsRelationDetailClientLocal(string authorizationHeader)
        {
        }


        public async Task UpdateAmountAsync(Guid OverdraftDetailID, decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied,
            Guid instanceID, Guid identityWorkID)
        {
            var mgr = new EmployeeConceptsRelationDetailManager();

            await mgr.UpdateAmountAsync(new UpdateAmountConceptDetailParams()
            {
                Amount = Amount,
                OverdraftDetailID = OverdraftDetailID,
                ConceptRelationsAmountApplied = ConceptRelationsAmountApplied,
                InstanceID = instanceID,
                IdentityWorkID = identityWorkID
            });
        }
    }
}
