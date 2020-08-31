using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class PreviewTransformParams : IdentityWorkParams
    {
        public Guid user { get; set; }

        public Guid InstanceID { get; set; }

        public PreviewTransformParams()
        {
            PreviewTransformParamsDetails = new List<PreviewTransformParamsDetail>();
        }

        [DataMember]
        public List<PreviewTransformParamsDetail> PreviewTransformParamsDetails { get; set; }

        [DataMember]
        public FiscalStampingVersion FiscalStampingVersion { get; set; }
    }

    [DataContract]
    public class PreviewTransformParamsDetail
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public Overdraft Overdraft { get; set; }

        [DataMember]
        public string XML { get; set; }
    }
}