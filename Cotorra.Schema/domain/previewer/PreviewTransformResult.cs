using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class PreviewTransformResult
    {
        public PreviewTransformResult()
        {
            PreviewTransformResultDetails = new List<PreviewTransformResultDetail>();
        }

        [DataMember]
        public List<PreviewTransformResultDetail> PreviewTransformResultDetails { get; set; }
    }

    [DataContract]
    public class PreviewTransformResultDetail
    {
        [DataMember]
        public string XML { get; set; }

        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public string TransformHTMLResult { get; set; }

        [DataMember]
        public byte[] TransformPDFResult { get; set; }
    }
}
