using CotorraNode.Common.Proxy;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Cotorra.Web
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public async Task Invoke(HttpContext context /* other dependencies */)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            var code = HttpStatusCode.InternalServerError; // 500 if unexpected

            if (ex is EntryPointNotFoundException) code = HttpStatusCode.NotFound;
            else if (ex is UnauthorizedAccessException) code = HttpStatusCode.Unauthorized;
            else if (ex is InvalidCastException) code = HttpStatusCode.BadRequest;

            ContentHelper contentHelper = new ContentHelper();
            contentHelper.Message = ex.Message;
            contentHelper.ServiceException = ex.ToString();
            contentHelper.StackTrace = ex.StackTrace;

            var result = JsonConvert.SerializeObject(contentHelper);
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
