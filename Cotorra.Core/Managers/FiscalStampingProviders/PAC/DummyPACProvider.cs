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
    public class DummyPACProvider : IPACProvider
    {
        public DummyPACProvider()
        {
        }

        public Task<CancelDocumentResult<ICFDINomProvider>> CancelStampingDocumentAsync(CancelDocumentResult<ICFDINomProvider> cancelDocumentResult)
        {
            throw new NotImplementedException();
        }

        public async Task<SignDocumentResult<ICFDINomProvider>> StampDocumetAsync(
            SignDocumentResult<ICFDINomProvider> signDocumentResult, FiscalStampingVersion fiscalStampingVersion, string xml)
        {
            //TFD
            var tfd = new Cotorra.Schema.CFDI33Nom12.TimbreFiscalDigital();
            tfd.FechaTimbrado = DateTime.Now;
            tfd.Leyenda = "";
            tfd.NoCertificadoSAT = "00001000000404486074";
            tfd.SelloCFD = "RIibW8tNhsl7aOeLKuyzi7dm5a2t+2FTsVjG4602lX2lcCQdMtosfwmJuBhUCYaYM9eNzcqtyXljEf0nsmsDI+ZL8LGMKmrMoiQDsgxaX0BZ77lU06d1SxweHYryR47qI+FwNogv1wqnWUiySlvs9H9LlAgJFZp2oAOaLLhaYwrSS62tjcULRUY8mev6ImXThLwzq6GESqzsHYL26bkzyRFtATXhhO8nQfYS8gB1o3JPkhdTvc7s7cdgjBIRreKc4cBxZZtkLnjx51NcVFvDqvfKdreKp45d+zlmpp/9lY65Irf/MY2/mlT4/aZyM/ZFC11HQ+KD/Pl3c4hfM1yGsQ==";
            tfd.SelloSAT = "Im6lw1+tKaaUdBK457eijgixdRYm39ch2hV0CLZxf0td6fx64C0HtmNEwJ8xw/WtSCD/VQMgsF8s3InJBcGUIUgIf4LYdd+sKdR2qzpr9IXk/+xDYBrQNaos8COUhH/LecB0el9EUYWLcnv6ranMDZHUlXpCo+iG7zx1S9LDc4AC1jQVrNzBL60EelSznmLr8Gw33HRu4AWjnWua+BWwrIXqzKvdO/PnaLiH/PhQUcGzHYjm5MC6cimAToF+gnN1yyy9h1yETI8kQtaaWNoTqK6f8QyMqe/jR+CD7JAVNCsHbDKR10iNCdVJzDgvcdmDFdrZp6ZyXIBWWAAWyAOldw==";
            tfd.RfcProvCertif = "MAS0810247C0";
            tfd.UUID = Guid.NewGuid().ToString().ToUpper();
            tfd.Version = "1.1";

            var complementos = (signDocumentResult.CFDI as Comprobante).Complemento.ToList();
            //complementos.Add(new ComprobanteComplemento() { Any = new XmlElement[1] { GetElement(CreateXmlNom(tfd)) } });
            (signDocumentResult.CFDI as Comprobante).Complemento = complementos.ToArray();

            signDocumentResult.UUID = Guid.Parse(tfd.UUID);

            return signDocumentResult;
        }
    }
}
