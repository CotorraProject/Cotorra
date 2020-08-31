using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using CotorraNode.Common.Library.Public;
using MoreLinq;
using Cotorra.Core.Managers.FiscalStampingProviders.PAC;
using Cotorra.Core.Utils;
using Cotorra.Core.Validator;
using Cotorra.Schema;
using Cotorra.Schema.CFDI33Nom12;
using RestSharp.Serializers;

namespace Cotorra.Core.Managers.FiscalStamping
{
    public class CancelStampingCFDIv33Nomv12 : ICancelStamping
    {
        private const string ALGORITHM_TRANSFORMATION = "http://www.w3.org/2000/09/xmldsig#enveloped-signature";
        private readonly Dictionary<string, string> statusCancelationCodes;

        public CancelStampingCFDIv33Nomv12()
        {
            /*  201 - El folio se ha cancelado con éxito.
                            202 - El CFDI ya había sido cancelado previamente.
                            203 - UUID no corresponde al emisor.
                            204 - El CFDI no aplica para cancelación.
                            205 - El UUID no existe o no ha sido procesado por el SAT.
                            402 - El Contribuyente no se encuentra el la LCO o la validez de obligaciones se reporta como negativa.*/
            statusCancelationCodes = new Dictionary<string, string>();
            statusCancelationCodes.Add("201", "El folio se ha cancelado con éxito.");
            statusCancelationCodes.Add("202", "El folio se ha cancelado con éxito.");
            statusCancelationCodes.Add("203", "UUID no corresponde al emisor.");
            statusCancelationCodes.Add("204", "El CFDI no aplica para cancelación.");
            statusCancelationCodes.Add("205", "El UUID no existe o no ha sido procesado por el SAT.");
            statusCancelationCodes.Add("402", "El Contribuyente no se encuentra el la LCO o la validez de obligaciones se reporta como negativa.");
        }

        /// <summary>
        /// creates the cancelacion xml 
        /// </summary>
        /// <param name="cancelDocumentParams"></param>
        /// <param name="cancelacion"></param>
        /// <returns></returns>
        private string createCancelationXML(CancelDocumentParams cancelDocumentParams, Cancelacion cancelacion)
        {
            //get FiscalInformation         
            X509Certificate2 x509Certificate2 = new X509Certificate2(cancelDocumentParams.CertificateCER);

            //get cancelacion xml
            string xml = SerializerXml.SerializeObject(cancelacion);
            System.Security.Cryptography.Xml.Signature signature = new Cotorra.DigitalSign.DigitalSign().ApplySignature(cancelDocumentParams.CertificateKey,
                cancelDocumentParams.Password, xml);
            cancelacion.Signature = new SignatureType();
            cancelacion.Signature.SignedInfo = new SignedInfoType();
            cancelacion.Signature.SignedInfo.CanonicalizationMethod = new CanonicalizationMethodType();
            cancelacion.Signature.SignedInfo.CanonicalizationMethod.Algorithm = signature.SignedInfo.CanonicalizationMethodObject.Algorithm;
            cancelacion.Signature.SignedInfo.SignatureMethod = new SignatureMethodType();
            cancelacion.Signature.SignedInfo.SignatureMethod.Algorithm = signature.SignedInfo.SignatureMethod;
            cancelacion.Signature.SignedInfo.Reference = new ReferenceType();
            Reference reference = (Reference)signature.SignedInfo.References[0];
            cancelacion.Signature.SignedInfo.Reference.URI = reference.Uri;
            cancelacion.Signature.SignedInfo.Reference.Transforms = new List<TransformType>()
              {
                new TransformType()
                {
                  Algorithm = ALGORITHM_TRANSFORMATION
                }
              }.ToArray();
            cancelacion.Signature.SignedInfo.Reference.DigestMethod = new DigestMethodType();
            cancelacion.Signature.SignedInfo.Reference.DigestMethod.Algorithm = reference.DigestMethod;
            cancelacion.Signature.SignedInfo.Reference.DigestValue = reference.DigestValue;
            cancelacion.Signature.SignatureValue = signature.SignatureValue;
            cancelacion.Signature.KeyInfo = new KeyInfoType();
            cancelacion.Signature.KeyInfo.X509Data = new X509DataType();
            cancelacion.Signature.SignedInfo.Reference.DigestValue = reference.DigestValue;
            cancelacion.Signature.SignatureValue = signature.SignatureValue;
            cancelacion.Signature.KeyInfo = new KeyInfoType();
            cancelacion.Signature.KeyInfo.X509Data = new X509DataType();
            cancelacion.Signature.KeyInfo.X509Data.X509IssuerSerial = new X509IssuerSerialType();
            cancelacion.Signature.KeyInfo.X509Data.X509IssuerSerial.X509IssuerName = x509Certificate2.Issuer;
            cancelacion.Signature.KeyInfo.X509Data.X509IssuerSerial.X509SerialNumber = x509Certificate2.GetSerialNumberString();
            cancelacion.Signature.KeyInfo.X509Data.X509Certificate = x509Certificate2.GetRawCertData();

            //get xml cancelacion
            string xmlCancelacion = SerializerXml.SerializeObject(cancelacion);
            return xmlCancelacion;
        }

        /// <summary>
        /// CancelDocumetAsync
        /// </summary>
        /// <returns></returns>
        public async Task<CancelPayrollStampingResult> CancelDocumetAsync(CancelDocumentParams cancelDocumentParams)
        {
            var cancelPayrollStampingResult = new CancelPayrollStampingResult();
            try
            {
                //Get datetime from ZipCode of Employer
                var zipCodeManager = new ZipCodeManager(cancelDocumentParams.ZipCodes);
                (var zipcode, var datetimeFromZipCode) = await zipCodeManager.GetZipCode(cancelDocumentParams.IssuerZipCode);

                //Fill the cancelation object
                Cancelacion cancelacion = new Cancelacion();
                cancelacion.RfcEmisor = cancelDocumentParams.IssuerRFC;
                cancelacion.Fecha = DateTime.Parse(string.Format("{0:s}", datetimeFromZipCode), CultureInfo.InvariantCulture);
                var cancelacionFolios = new List<CancelacionFolios>();
                var uuids = cancelDocumentParams.CancelDocumentParamsDetails.Select(p => p.UUID).ToList();
                uuids.ForEach(p => cancelacionFolios.Add(new CancelacionFolios() { UUID = p.ToString().ToLower() }));
                cancelacion.Folios = cancelacionFolios.ToArray();

                //Creates the cancelationXML
                var xmlCancelation = createCancelationXML(cancelDocumentParams, cancelacion);
               
                //Call PAC for cancelation
                var stampResult = new CancelDocumentResult<ICFDINomProvider>();
                stampResult.CancelationXML = xmlCancelation;
                stampResult.InstanceID = cancelDocumentParams.InstanceID;
                IPACProvider pACProvider = FactoryPACProvider.CreateInstanceFromConfig();
                var cancelationPACResult = await pACProvider.CancelStampingDocumentAsync(stampResult);

                if (cancelationPACResult.WithErrors)
                {
                    cancelPayrollStampingResult.WithErrors = true;
                    cancelPayrollStampingResult.Message = cancelationPACResult.Details;
                }
                else
                {
                    cancelPayrollStampingResult.WithErrors = false;
                    cancelPayrollStampingResult.CancelacionXMLRequest = cancelationPACResult.CancelationXML;
                    cancelPayrollStampingResult.CancelacionXMLAcknowledgeResponse = cancelationPACResult.CancelationAcknowledgmentReceipt;

                    //Fill each UUID with the proper status
                    var acuse = SerializerXml.DeserializeObject<Acuse>(cancelationPACResult.CancelationAcknowledgmentReceipt);
                    acuse.Folios.ForEach(p =>
                    {
                        var detail = new CancelPayrollStampingResultDetail();
                        /*
                        201 - El folio se ha cancelado con éxito.
                        202 - El CFDI ya había sido cancelado previamente.
                        203 - UUID no corresponde al emisor.
                        204 - El CFDI no aplica para cancelación.
                        205 - El UUID no existe o no ha sido procesado por el SAT.
                        402 - El Contribuyente no se encuentra el la LCO o la validez de obligaciones se reporta como negativa.
                        */
                        if (p.EstatusUUID == "201" || p.EstatusUUID == "202") //estatus good
                        {
                            statusCancelationCodes.TryGetValue(p.EstatusUUID, out string message);
                            detail.Message = $"{p.EstatusUUID} : {message}";
                            detail.UUID = Guid.Parse(p.UUID);
                            detail.PayrollStampingResultStatus = PayrollStampingResultStatus.Success;
                        }
                        else
                        {
                            statusCancelationCodes.TryGetValue(p.EstatusUUID, out string message);
                            detail.Message = $"Error al cancelar {p.EstatusUUID} : {message}";
                            detail.UUID = Guid.Parse(p.UUID);
                            detail.PayrollStampingResultStatus = PayrollStampingResultStatus.Fail;
                        }
                        cancelPayrollStampingResult.CancelPayrollStampingResultDetails.Add(detail);
                    });
                }
            }
            catch (Exception ex)
            {
                cancelPayrollStampingResult.WithErrors = true;
                cancelPayrollStampingResult.Message = $"Ocurrió un error no controlado en la cancelación: {ex.Message}";
            }

            return cancelPayrollStampingResult;
        }
    }
}
