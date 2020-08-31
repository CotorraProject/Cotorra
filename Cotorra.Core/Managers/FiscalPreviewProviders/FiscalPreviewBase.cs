using Cotorra.Core.Utils;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Cotorra.Schema;

namespace Cotorra.Core.Managers.FiscalPreview
{
    public abstract class FiscalPreviewBase
    {
        public abstract string XsltPath { get; set; }

        public abstract string URLSAT { get; set; }

        public virtual string GetQRCodeWithTemplate(XMLGet xml)
        {
            string cbbSrc = string.Empty;
            try
            {
                var selloTrunc = xml.SelectSingleNode("cfdi:Comprobante/@Sello").Value;
                selloTrunc = selloTrunc.Substring(selloTrunc.Length - 8);
                cbbSrc = URLSAT +
                    "&id=" + xml.SelectSingleNode("cfdi:Comprobante/cfdi:Complemento/tfd:TimbreFiscalDigital/@UUID").Value +
                    "&re=" + xml.SelectSingleNode("cfdi:Comprobante/cfdi:Emisor/@Rfc").Value +
                    "&rr=" + xml.SelectSingleNode("cfdi:Comprobante/cfdi:Receptor/@Rfc").Value +
                    "&tt=" + xml.SelectSingleNode("cfdi:Comprobante/@Total").Value +
                    "&fe=" + selloTrunc;
                return cbbSrc;
            }
            catch (Exception)
            {
                return cbbSrc;
            }
        }

        public virtual async Task<GetPreviewCancelationAckURLResult> GetPreviewCancelationAckURLAsync(Guid cancelationResponseXMLID, Guid instanceID)
        {
            GetPreviewCancelationAckURLResult result = new GetPreviewCancelationAckURLResult();
            result.CancelationResponseXMLID = cancelationResponseXMLID;

            BlobStorageUtil blobManagerUtil = new BlobStorageUtil(instanceID);
            await blobManagerUtil.InitializeAsync();

            //check if XML exists
            if (await blobManagerUtil.ExistsFile($"{cancelationResponseXMLID.ToString()}.xml"))
            {
                result.XmlAcknowledgementUri = blobManagerUtil.GetBlobSasUri(instanceID, $"{cancelationResponseXMLID.ToString()}.xml");
            }
            else
            {
                throw new CotorraException(109, "109", "No fue posible recuperar el XML del acuse de cancelación.", null);
            }

            return result;
        }



    }
}
