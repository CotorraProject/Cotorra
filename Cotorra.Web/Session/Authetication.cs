using Microsoft.AspNetCore.Mvc.Filters;

namespace Cotorra.Web.Utils
{
    public class Authentication : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            SessionModel.Initialize();
        }
    }
}
