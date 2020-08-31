using CotorraNode.Common.ManageException;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;

namespace Cotorra.Web.Utils
{
    public class TelemetryUIAttribute : ExceptionFilterAttribute
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IModelMetadataProvider _modelMetadataProvider;

        public TelemetryUIAttribute(IHostingEnvironment hostingEnvironment, IModelMetadataProvider modelMetadataProvider)
        {
            _hostingEnvironment = hostingEnvironment;
            _modelMetadataProvider = modelMetadataProvider;
        }

        public TelemetryUIAttribute()
        {

        }

        public async override void OnException(ExceptionContext context)
        {
            var baseEx = new BaseException("", context.Exception.Message, context.Exception);
            try
            {
                if (context.Exception is BaseException)
                {
                    baseEx = context.Exception as CotorraNode.Common.ManageException.BaseException;
                }

                var message = baseEx.Message;
                if (baseEx.Message.Contains("One or more errors occurred. ("))
                {
                    message = message.Replace("One or more errors occurred. (", "");
                    message = message.Replace("})", "}");
                    message = message.Replace(")", "");
                }
                baseEx = new BaseException(baseEx.Code, message, baseEx);
            }
            catch (Exception ex)
            {
                baseEx = new BaseException(baseEx.Code, ex.Message, baseEx);
            }

            context.Result = new JsonResult(CotorraNode.App.Common.UX.Tools.GetMessageFromException(baseEx));
        }

    }
}
