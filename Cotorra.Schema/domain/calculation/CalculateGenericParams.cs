using CotorraNode.Common.Base.Schema.Parameters.Base;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{
    [DataContract]
    public class CalculateGenericParams : IdentityWorkParams, ICalculateParams
    {
        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public string Formula { get; set; }

        [DataMember]
        public Guid? OverdraftID { get; set; }
    }
}
