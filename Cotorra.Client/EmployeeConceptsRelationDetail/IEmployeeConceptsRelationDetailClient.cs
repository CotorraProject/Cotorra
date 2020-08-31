using Cotorra.Schema; 
using System;
using System.Collections.Generic; 
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public interface IEmployeeConceptsRelationDetailClient
    {
        Task UpdateAmountAsync(Guid OverdraftDetailID, Decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied,
             Guid instanceID, Guid identityWorkID);
    }
}
