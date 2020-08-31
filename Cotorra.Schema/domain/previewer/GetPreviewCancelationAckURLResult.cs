using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class GetPreviewCancelationAckURLResult
    {
        [DataMember]
        public Guid CancelationResponseXMLID { get; set; }

        [DataMember]
        public string XmlAcknowledgementUri { get; set; }
    }
}
