using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CancelDocumentParams : IdentityWorkParams
    {
        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid user { get; set; }

        [DataMember]
        public string IssuerRFC { get; set; }

        [DataMember]
        public string IssuerZipCode { get; set; }

        [DataMember]
        public List<catCFDI_CodigoPostal> ZipCodes { get; set; }
     
        [DataMember]
        public byte[] CertificateCER { get; set; }

        [DataMember]
        public byte[] CertificateKey { get; set; }

        [DataMember]
        public string Password { get; set; }

        [DataMember]
        public List<CancelDocumentParamsDetail> CancelDocumentParamsDetails { get; set; }
    }

    [DataContract]
    public class CancelDocumentParamsDetail
    {
        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public Guid OverdraftID { get; set; }
    }
}
