using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class GetPreviewResult
    {
        [DataMember]
        public PreviewTransformResult PreviewTransformResult { get; set; }
    }
}
