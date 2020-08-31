using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy; 
using Cotorra.ClientProxy; 
using Cotorra.Schema; 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cotorra.Client
{
    public class EmployeeConceptsRelationDetailClientProxy : IEmployeeConceptsRelationDetailClient
    {
        private readonly string _authorizationHeader;
        private readonly string _cotorraUri;
        public EmployeeConceptsRelationDetailClientProxy(string authorizationHeader)
        {
            _cotorraUri = $"{ConfigManager.GetValue("CotorraService")}api/EmployeeConceptsRelation";
            _authorizationHeader = authorizationHeader;
        }


        public async Task UpdateAmountAsync(Guid OverdraftDetailID, decimal Amount, List<ConceptRelationUpdateAmountDTO> ConceptRelationsAmountApplied,
            Guid instanceID, Guid identityWorkID)
        {

            var parameters = new UpdateAmountConceptDetailParams()
            {
                Amount = Amount,
                OverdraftDetailID = OverdraftDetailID,
                ConceptRelationsAmountApplied = ConceptRelationsAmountApplied,
                InstanceID = instanceID,
                IdentityWorkID = identityWorkID
            };

            var result = await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, _authorizationHeader,
               new Uri($"{_cotorraUri}/CalculateFormula"), new object[] { parameters }).ContinueWith((i) =>
               {
                   if (i.Exception != null)
                   {
                       throw i.Exception;
                   }

                   return i.Result;
               });


            return;
        }
    }



}
