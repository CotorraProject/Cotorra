using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Private;
using CotorraNode.Layer2.Company.Client;
using CotorraNode.Platform.Layer2.Company.Schema.Results;
using Cotorra.Core.Validator;
using Cotorra.Schema;

namespace Cotorra.Core
{
    public class EmployeeConceptsRelationDetailManager
    {
        MiddlewareManager<EmployeeConceptsRelationDetail> _mgrEmployeeConceptsRelationDetail;
        MiddlewareManager<OverdraftDetail> _clientOverdraftDetail;

        public EmployeeConceptsRelationDetailManager()
        {

            _mgrEmployeeConceptsRelationDetail = new MiddlewareManager<EmployeeConceptsRelationDetail>(
                      new BaseRecordManager<EmployeeConceptsRelationDetail>(),
                      new EmployeeConceptsRelationDetailValidator()
                      );

            _clientOverdraftDetail = new MiddlewareManager<OverdraftDetail>(
          new BaseRecordManager<OverdraftDetail>(),
          new OverdraftDetailValidator()
          );


        }

        public async Task UpdateAmountAsync(UpdateAmountConceptDetailParams parameters)
        {
            Guid OverdraftDetailID = parameters.OverdraftDetailID;
            Decimal Amount = parameters.Amount;
            List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied = parameters.ConceptRelationsAmountApplied;
            Guid identityWorkID = parameters.IdentityWorkID;
            Guid instanceID = parameters.InstanceID;


            var ids = ConceptRelationsAmountApplied.Select(x => x.ID);
            var relationDetailDB = _mgrEmployeeConceptsRelationDetail.FindByExpressionAsync(x => ids.Contains(x.ID) && x.InstanceID == instanceID, identityWorkID);
            var overdraftdetailDB = _clientOverdraftDetail.FindByExpressionAsync(x => x.ID == OverdraftDetailID && x.InstanceID == instanceID, identityWorkID);

            var details = (await relationDetailDB);
            details.ForEach(item =>
            {
                item.AmountApplied = ConceptRelationsAmountApplied.FirstOrDefault(x => x.ID == item.ID) != null ? ConceptRelationsAmountApplied.FirstOrDefault(x => x.ID == item.ID).AmountApplied : 0;
                item.IsAmountAppliedCapturedByUser = ConceptRelationsAmountApplied.FirstOrDefault(x => x.ID == item.ID) != null ? ConceptRelationsAmountApplied.FirstOrDefault(x => x.ID == item.ID).IsAmountAppliedCapturedByUser : false;
            });

            var overDetails = (await overdraftdetailDB);
            overDetails.ForEach(item =>
            {
                item.Amount = Amount;
                item.IsTotalAmountCapturedByUser = details.Any(x => x.IsAmountAppliedCapturedByUser);
            });
           

            using (var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _mgrEmployeeConceptsRelationDetail.UpdateAsync(details, identityWorkID);
                await _clientOverdraftDetail.UpdateAsync(overDetails, identityWorkID);
                scope.Complete();
            }

        }

    }

}
