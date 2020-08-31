using CotorraNode.Common.Config;
using CotorraNode.Common.Library.Public;
using CotorraNode.Common.Proxy;
using CotorraNube.CommonApp.RestClient;
using Cotorra.Core.Managers.FiscalStamping;
using Cotorra.Core.Utils;
using Cotorra.Schema;
using Cotorra.Schema.CFDI33Nom12;
using Cotorra.Schema.pac;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using RestClient = RestSharp.RestClient;

namespace Cotorra.Core.Managers.FiscalStampingProviders.PAC
{
    public class COTORRAiPACProvider : IPACProvider
    {
        private const string VERSION = "1.0";
        private const string PAC_DOCUMENT_TYPE = "CFDI_3_3";
        private const string PAC_ATTRIBUTE_TITLE_RFC = "RFC";
        private const string PAC_ATTRIBUTE_TITLE_PRODUCT = "Producto";
        private const string PAC_ATTRIBUTE_VALUE_PRODUCT = "StampingAPI";
        private const string PAC_ATTRIBUTE_TITLE_PRODUCT_VERSION = "VersionProducto";
        private const string PAC_ATTRIBUTE_VALUE_PRODUCT_VERSION = "2.0.0";
        private const string PAC_ATTRIBUTE_TITLE_PRODUCT_TYPE = "TipoProducto";
        private const string PAC_ATTRIBUTE_VALUE_PRODUCT_TYPE = "Aplicacion";
        private readonly string CFDITokenKey;
        private readonly string StampingUri;

        public COTORRAiPACProvider()
        {
            CFDITokenKey = ConfigManager.GetValue("CFDITokenKey");
            //StampingUri = ConfigManager.GetValue("StampingService");
        }

        public Task<CancelDocumentResult<ICFDINomProvider>> CancelStampingDocumentAsync(CancelDocumentResult<ICFDINomProvider> cancelDocumentResult)
        {
            throw new NotImplementedException();
        }

        public async Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(
            SignDocumentResult<ICFDINomProvider> signDocumentResult, FiscalStampingVersion fiscalStampingVersion, string xml)
        {
            var stampingResult = String.Empty;

            var stampingRequest_CFDI33Nom12 = new StampingRequest_CFDI33Nom12();
            stampingRequest_CFDI33Nom12.Version = VERSION;
            stampingRequest_CFDI33Nom12.PACDocumentType = PAC_DOCUMENT_TYPE;
            stampingRequest_CFDI33Nom12.Body = new CFDIRequest_CFDI33Nom12()
            {
                XmlString = xml
            };
            stampingRequest_CFDI33Nom12.AdditionalInformation.Add(new AdditionalInformation_CFDI33Nom12(PAC_ATTRIBUTE_TITLE_RFC, signDocumentResult.EmployerRFC));
            stampingRequest_CFDI33Nom12.AdditionalInformation.Add(new AdditionalInformation_CFDI33Nom12(PAC_ATTRIBUTE_TITLE_PRODUCT, PAC_ATTRIBUTE_VALUE_PRODUCT));
            stampingRequest_CFDI33Nom12.AdditionalInformation.Add(new AdditionalInformation_CFDI33Nom12(PAC_ATTRIBUTE_TITLE_PRODUCT_VERSION, PAC_ATTRIBUTE_VALUE_PRODUCT_VERSION));
            stampingRequest_CFDI33Nom12.AdditionalInformation.Add(new AdditionalInformation_CFDI33Nom12(PAC_ATTRIBUTE_TITLE_PRODUCT_TYPE, PAC_ATTRIBUTE_VALUE_PRODUCT_TYPE));
            stampingRequest_CFDI33Nom12.Token = "";

            //call service async
            await ServiceHelperExtensions.CallRestServiceAsync(Format.JSON, RestMethod.POST, null,
                    new Uri($"{StampingUri}"), new object[] { stampingRequest_CFDI33Nom12 })
                    .ContinueWith((i) =>
                    {
                        if (i.Result.Contains("FAIL") || i.Result.Contains("An error has occurred"))
                        {
                            throw new CotorraException(90001, "90001", i.Result, null);
                        }

                        stampingResult = i.Result;
                    });

            var stampingResult_CFDI33Nom12 = JsonSerializer.DeserializeObject<StampingResult_CFDI33Nom12>(stampingResult);

            //TFD
            var responseList = stampingResult_CFDI33Nom12.ResponseList_CFDI33Nom12.FirstOrDefault();
            var tfd = SerializerXml.DeserializeObject<TimbreFiscalDigital>(responseList.ResponseValue);

            var complementos = (signDocumentResult.CFDI as Comprobante).Complemento.ToList();
            //complementos.Add(new ComprobanteComplemento() { Any = new XmlElement[1] { GetElement(CreateXmlNom(tfd)) } });
            (signDocumentResult.CFDI as Comprobante).Complemento = complementos.ToArray();

            signDocumentResult.UUID = Guid.Parse(tfd.UUID);

            return signDocumentResult;
        }
    }
}
