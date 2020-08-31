using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CalculationFireAndForgetByPeriodParams : IdentityWorkParams
    {
        [DataMember]
        public List<Guid> PeriodIds { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }
    }
}
