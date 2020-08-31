using System;
using System.Threading.Tasks;
using CotorraNode.Common.Service.Provisioning.API;
using Microsoft.AspNetCore.Mvc;
using Cotorra.Core;
using Cotorra.Core.Managers.FiscalPreview;
using Cotorra.Schema;
using Cotorra.Schema.DTO.Catalogs;

namespace Cotorra.WebAPI.Controllers.Preview
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreviewController : BaseCotorraController
    {
        [HttpGet("GetPreviewUrlByUUIDAsync/{instanceId}/{uuid}")]
        [Security(ServiceID = PermissionContants.SuperMamalonPermission.ServiceID,
        UseAuthorization = false,
        UseSession = false,
        Permissions = new[] { PermissionContants.SuperMamalonPermission.PermissionID },
        ResourceID = PermissionContants.SuperMamalonPermission.CotoRRA_Cloud_ID_String,
        UserInstanceAsOwner = false)]
        public async Task<ActionResult<string>> GetPreviewUrlByUUIDAsync(Guid instanceId, Guid uuid)
        {
            IFiscalPreview fiscalPreview = FiscalPreviewFactory.CreateInstance(FiscalStampingVersion.CFDI33_Nom12);
            var resultPreview = await fiscalPreview.GetPreviewUrlByUUIDAsync(instanceId, uuid);
            return new JsonResult(resultPreview);
        }

        [HttpGet("GetPreviewByOverdraft/{companyId}/{instanceId}/{overdraftId}")]
        [Security(ServiceID = PermissionContants.SuperMamalonPermission.ServiceID,
         UseAuthorization = false,
         UseSession = false,
         Permissions = new[] { PermissionContants.SuperMamalonPermission.PermissionID },
         ResourceID = PermissionContants.SuperMamalonPermission.CotoRRA_Cloud_ID_String,
         UserInstanceAsOwner = false)]
        public async Task<ActionResult<string>> GetPreviewByOverdraft(Guid companyId, Guid instanceId, Guid overdraftId)
        {
            var employeeDTO = new EmployeeDTO();
            JsonResult result = new JsonResult(employeeDTO);
            try
            {
                GetPreviewResult getPreviewResult = new GetPreviewResult();

                var fiscalPreviewManager = new FiscalPreviewerManager();
                var previewTransformParams = new PreviewTransformParams();
                previewTransformParams.FiscalStampingVersion = FiscalStampingVersion.CFDI33_Nom12;
                previewTransformParams.InstanceID = instanceId;
                previewTransformParams.IdentityWorkID = companyId;
                previewTransformParams.PreviewTransformParamsDetails.Add(new PreviewTransformParamsDetail()
                {
                    OverdraftID = overdraftId
                });


                //Transforma el XML a HTML y PDF
                getPreviewResult.PreviewTransformResult = await fiscalPreviewManager.TransformAsync(previewTransformParams);
                return new JsonResult(getPreviewResult.PreviewTransformResult);
            }
            catch (Exception ex)
            {
                if (ex != null)
                {
                    var exception = ex;
                    while (exception.Message.Contains("Exception has been thrown by the target of an invocation"))
                    {
                        exception = exception.InnerException;
                    }

                    throw exception;
                }
            }
            return result;
        }

    }
}
