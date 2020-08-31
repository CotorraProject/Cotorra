using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class JobPosition : IdentityCatalogEntityExt
    {
        [DataMember]
        public JobPositionRiskType JobPositionRiskType { get; set; }

    }
}
