using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class GetPreviewUrlResult
    {
        [DataMember]
        public Guid UUID { get; set; }

        [DataMember]
        public string XMLUri { get; set; }

        [DataMember]
        public string PDFUri { get; set; }
    }
}
