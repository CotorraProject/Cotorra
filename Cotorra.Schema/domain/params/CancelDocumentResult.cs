using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CancelDocumentResult<T> : IdentityWorkParams where T : ICFDINomProvider
    {

        [DataMember]
        public Guid user { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }      

        [DataMember]
        public string CancelationAcknowledgmentReceipt { get; set; }       

        [DataMember]
        public string CancelationXML { get; set; }

        [DataMember]
        public bool WithErrors { get; set; }

        [DataMember]
        public string Details { get; set; }
    }
}
