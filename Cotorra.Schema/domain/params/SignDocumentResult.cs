using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class SignDocumentResult<T> : IdentityWorkParams where T : ICFDINomProvider
    {
        public SignDocumentResult()
        {           
        }

        [DataMember]
        public Guid user { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public string EmployerRFC { get; set; }

        [DataMember]
        public string CertificateB64 { get; set; }

        [DataMember]
        public string CertificateNumber { get; set; }

        [DataMember]
        public string SignString { get; set; }

        [DataMember]
        public string OriginalString { get; set; }

        [DataMember]
        public string TFD { get; set; }

        [DataMember]
        public string CancelationAcknowledgmentReceipt { get; set; }

        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public T CFDI { get; set; }

        [DataMember]
        public string XML { get; set; }

        [DataMember]
        public bool WithErrors { get; set; }

        [DataMember]
        public string Details { get; set; }
    }
}
