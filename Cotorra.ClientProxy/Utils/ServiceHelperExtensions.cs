using System;
using System.Net;
using System.Reflection;
using System.Text;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using CotorraNode.Common.Proxy;
using CotorraNode.Common.ManageException.Exceptions;

namespace Cotorra.ClientProxy
{
    public static class ServiceHelperExtensions
    {
        private static readonly HashSet<Uri> _endpoints = new HashSet<Uri>();

        public static async Task<string> CallRestServiceAsync(Format restType, RestMethod restConstant, string authorizationHeader, Uri requestUri, params object[] parameters)
        {
            var jsonObject = GetParameters(parameters);
            return await CallServiceAsync(restType, restConstant, authorizationHeader, requestUri, jsonObject.ToString());
        }

        public static async Task<string> CallRestServiceAsync(Format restType, RestMethod restConstant, string authorizationHeader, Uri requestUri, Guid identityWorkID,
            Guid instanceID, params object[] parameters)
        {
            var jsonObject = GetParameters(parameters);
            return await CallServiceAsync(restType, restConstant, authorizationHeader, requestUri, jsonObject.ToString(), "", identityWorkID, instanceID);
        }

        private static async Task<string> CallServiceAsync(Format restType, RestMethod restConstant, string authorizationHeader, Uri requestUri, string serializedParameters, string TransactionKey = "",
            Guid? identityWorkID = null, Guid? instanceID = null )
        {
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(40);
            string result = string.Empty;
            ConfigureEndPoints(requestUri);
            HttpRequestMessage reqmsg = new HttpRequestMessage();
            reqmsg.RequestUri = requestUri;
            reqmsg.Method = new HttpMethod(restConstant.ToString("g"));
            reqmsg.Headers.TryAddWithoutValidation(ServiceHelperConstants.authorizationHeaderTitle, authorizationHeader);
            reqmsg.Headers.TryAddWithoutValidation(ServiceHelperConstants.transactionHeaderTitle, TransactionKey);

            if (identityWorkID != null)
            {
                reqmsg.Headers.TryAddWithoutValidation("companyid", identityWorkID.ToString());
            }
            if (instanceID != null)
            {
                reqmsg.Headers.TryAddWithoutValidation("instanceid", instanceID.ToString());
            }

            if (reqmsg.Method == HttpMethod.Post || reqmsg.Method == HttpMethod.Put)
            {
                reqmsg.Content = new StringContent(serializedParameters, Encoding.UTF8, resolveContentType(restType));
            }
            var response = await client.SendAsync(reqmsg, HttpCompletionOption.ResponseContentRead);
            result = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                if (!String.IsNullOrEmpty(result))
                {
                    try
                    {
                        var contentHelper = Newtonsoft.Json.JsonConvert.DeserializeObject<ContentHelper>(result);
                        manageWebException(contentHelper);
                    }
                    catch (Newtonsoft.Json.JsonReaderException ex)
                    {
                        throw new CallRestException().getDefault(ex.TargetSite, result, (int)response.StatusCode);
                    }
                }
                else
                {
                    throw new CallRestException().getDefault(MethodBase.GetCurrentMethod(), "No se pudo conectar al servicio.", (int)response.StatusCode);
                }
            }
            else
            {
                result = await response.Content.ReadAsStringAsync();
            }
            return result;
        }

        private static void manageWebException(ContentHelper contentHelper)
        {
            var callRestException = new CallRestException().getDefault(MethodBase.GetCurrentMethod(), $"Message: {contentHelper.Message}{Environment.NewLine}Detail: {contentHelper.ServiceException}");
            var returnException = callRestException as CallRestException;
            returnException.StackTraceFromService = contentHelper.StackTrace;
            //  returnException.ServiceExceptionDetail =   JsonConvert.DeserializeObject<ServiceException>(contentHelper.ServiceException);
            returnException.ServiceExceptionDetail = System.Text.Json.JsonSerializer.Deserialize<ServiceException>(contentHelper.ServiceException);
            returnException.ErrorCode = contentHelper.ErrorCode;
            returnException.TransactionID = contentHelper.TransactionID;
            throw returnException;
        }

        private static StringBuilder GetParameters(object[] parameters)
        {
            StringBuilder jsonObject = new StringBuilder();
            if (!(parameters == null || parameters.Length == 0))
            {
                for (int i = 0; i < parameters.Length; i++)
                {
                    if (parameters[i] != null)
                    {
                        jsonObject.Append(System.Text.Json.JsonSerializer.Serialize(parameters[i]));
                        //jsonObject.Append(JsonConvert.SerializeObject(parameters[i]));
                        if (i < parameters.Length - 1)
                        {
                            jsonObject.Append(",");
                        }
                    }
                }
            }
            return jsonObject;
        }

        private static void ConfigureEndPoints(Uri uri)
        {
            lock (_endpoints)
            {
                if (_endpoints.Contains(uri)) { return; }
                var sp = ServicePointManager.FindServicePoint(uri);
                sp.ConnectionLimit = ServiceHelper.DefaultConnectionLimit;
                sp.ConnectionLeaseTimeout = (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
                _endpoints.Add(uri);
            }


        }

        private static string resolveContentType(Format restType)
        {
            string result = String.Empty;
            switch (restType)
            {
                case Format.JSON: result = string.Format("{0}/json", ServiceHelperConstants.postFix); break;
                case Format.XML: result = string.Format("{0}/xml", ServiceHelperConstants.postFix); break;
            }
            return result;
        }

    }
}
