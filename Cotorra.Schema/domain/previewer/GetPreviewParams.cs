using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;


namespace Cotorra.Schema
{
    [DataContract]
    public class GetPreviewParams : IdentityWorkParams
    {
        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public FiscalStampingVersion FiscalStampingVersion { get; set; }

        [DataMember]
        public Guid OverdraftID { get; set; }
    }
}
