using CotorraNode.Common.Base.Schema.Parameters.Base;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class GenerateSettlementLetterParams : IInstanceIDParams
    {
        [DataMember]
        public List<Guid> OverdraftIDs { get; set; }

        [DataMember]
        public string Token { get; set; }


    }


}
