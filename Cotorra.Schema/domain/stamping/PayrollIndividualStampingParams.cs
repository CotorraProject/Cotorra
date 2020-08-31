using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class PayrollIndividualStampingParams : PayrollStampingParams
    {
        [DataMember]
        public Guid OverdraftID { get; set; }
    }
}
