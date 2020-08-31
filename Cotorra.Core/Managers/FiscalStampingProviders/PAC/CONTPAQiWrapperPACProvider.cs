using CotorraNode.Common.Config;
using CotorraNode.Common.Proxy;
using Newtonsoft.Json;
using Cotorra.Core.Managers.FiscalStamping;
using Cotorra.Core.Managers.FiscalValidator;
using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Cotorra.Core.Managers.FiscalStampingProviders.PAC
{
    public class COTORRAiWrapperPACProvider : IPACProvider
    {
        [DataContract]
        public class CFDIWrapperResult
        {
            [DataMember]
            public string status { get; set; }

            [DataMember]
            public string title { get; set; }

            [DataMember]
            public string detail { get; set; }

            [DataMember]
            public string traceId { get; set; }

            [DataMember]
            public List<string> errors { get; set; }
        }

        private readonly string _cotorraiWrapperPACUrl;
        private readonly string _cotorraiWrapperCFDIToken;
        private readonly HashSet<Uri> _endpoints = new HashSet<Uri>();

        public COTORRAiWrapperPACProvider()
        {
            _cotorraiWrapperPACUrl = $"{ConfigManager.GetValue("StampingCOTORRAiWrapper")}";
            _cotorraiWrapperCFDIToken = ConfigManager.GetValue("CFDITokenKey");
        }

        private void ConfigureEndPoints(Uri uri)
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

        private string resolveContentType(Format restType)
        {
            string result = String.Empty;
            switch (restType)
            {
                case Format.JSON: result = string.Format("{0}/json", ServiceHelperConstants.postFix); break;
                case Format.XML: result = string.Format("{0}/xml", ServiceHelperConstants.postFix); break;
            }
            return result;
        }

        private async Task<CFDIWrapperResult> CallServiceAsync(Format restType,
            RestMethod restConstant, string token, Uri requestUri,
            string serializedParameters)
        {
            CFDIWrapperResult cFDIWrapperResult = new CFDIWrapperResult() { status = "400", detail = "Error no controlado" };
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromMinutes(40);
            string result = string.Empty;
            ConfigureEndPoints(requestUri);
            HttpRequestMessage reqmsg = new HttpRequestMessage();
            reqmsg.RequestUri = requestUri;
            reqmsg.Method = new HttpMethod(restConstant.ToString("g"));
            reqmsg.Headers.TryAddWithoutValidation("token", token);

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
                    cFDIWrapperResult = JsonConvert.DeserializeObject<CFDIWrapperResult>(result);
                }
                else
                {
                    throw new CotorraException(1000, "1000", "No se pudo conectar al servicio.", null);
                }
            }
            else
            {
                cFDIWrapperResult.detail = await response.Content.ReadAsStringAsync();
            }

            return cFDIWrapperResult;
        }

        public async Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(
            SignDocumentResult<ICFDINomProvider> signDocumentResult, FiscalStampingVersion fiscalStampingVersion, string xml)
        {
            //call service async
            CFDIWrapperResult result = null;
            var xmlSerialized = JsonConvert.SerializeObject(xml); 

            await CallServiceAsync(Format.JSON, RestMethod.POST, _cotorraiWrapperCFDIToken,
                  new Uri($"{_cotorraiWrapperPACUrl}/fiscal/stamping/stamp"), xmlSerialized)
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            if (result.status == "400" && result.errors == null)
            {
                //TFD Fix string input :(
                var xmlTFD = result.detail.Remove(0, 1);
                xmlTFD = xmlTFD.Remove(xmlTFD.Length - 1, 1);
                xmlTFD = xmlTFD.Replace("\\\"", "\"");
                signDocumentResult.WithErrors = false;
                signDocumentResult.TFD = xmlTFD;
            }
            else if (result.status == "400" && result.errors != null)
            {
                signDocumentResult.WithErrors = true;
                signDocumentResult.Details = result.errors.FirstOrDefault();

            }
            else
            {
                signDocumentResult.WithErrors = true;
                signDocumentResult.Details = $"Ocurrió un error no esperado en el timbrado: {result.detail}";
            }

            return signDocumentResult;
        }

        public async Task<CancelDocumentResult<ICFDINomProvider>> CancelStampingDocumentAsync(
            CancelDocumentResult<ICFDINomProvider> cancelDocumentResult)
        {
            //call service async
            CFDIWrapperResult result = null;
            var xmlSerialized = JsonConvert.SerializeObject(cancelDocumentResult.CancelationXML);

            await CallServiceAsync(Format.JSON, RestMethod.POST, _cotorraiWrapperCFDIToken,
                  new Uri($"{_cotorraiWrapperPACUrl}/fiscal/stamping/cancelrequest"), xmlSerialized)
                .ContinueWith((i) =>
                {
                    if (i.Exception != null)
                    {
                        throw i.Exception;
                    }

                    result = i.Result;
                });

            if (result.status == "400" && result.errors == null)
            {
                //TFD Fix string input :(
                var xmlAcknowledgement = result.detail.Remove(0, 1);
                xmlAcknowledgement = xmlAcknowledgement.Remove(xmlAcknowledgement.Length - 1, 1);
                xmlAcknowledgement = xmlAcknowledgement.Replace("\\\"", "\"");
                xmlAcknowledgement = xmlAcknowledgement.Replace("\\r\\n", "");
                cancelDocumentResult.WithErrors = false;
                cancelDocumentResult.CancelationAcknowledgmentReceipt = xmlAcknowledgement;
            }
            else if (result.status == "400" && result.errors != null)
            {
                cancelDocumentResult.WithErrors = true;
                cancelDocumentResult.Details = result.errors.FirstOrDefault();

            }
            else
            {
                cancelDocumentResult.WithErrors = true;
                cancelDocumentResult.Details = $"Ocurrió un error no esperado en el timbrado: {result.detail}";
            }

            return cancelDocumentResult;
        }
    }
}
