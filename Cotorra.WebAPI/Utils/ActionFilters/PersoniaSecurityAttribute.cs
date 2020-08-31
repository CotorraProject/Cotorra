using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Cotorra.WebAPI.Controllers;
using System;
using System.Collections.Generic;

namespace Cotorra.WebAPI.ActionFilters
{

    internal class CotorriaSecurityAttribute : ActionFilterAttribute
    {

        public override void OnActionExecuting(ActionExecutingContext actionContext)
        {
            actionContext.HttpContext.Request.Headers.TryGetValue("companyid", out StringValues companyid);
            actionContext.HttpContext.Request.Headers.TryGetValue("instanceid", out StringValues instanceid);
            actionContext.HttpContext.Request.Headers.TryGetValue("userid", out StringValues userid);
            var controller = (actionContext.Controller as BaseCotorraController);

            Guid.TryParse(companyid.ToString(), out Guid identityWorkID);
            controller.IdentityWorkID = identityWorkID;
            Guid.TryParse(instanceid.ToString(), out Guid instanceID);
            controller.InstanceID = instanceID;
            Guid.TryParse(userid.ToString(), out Guid identityID);
            controller.IdentityID = identityID;
        }
    }
}
