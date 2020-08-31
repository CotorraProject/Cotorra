using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class UMI : BaseEntity
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public Decimal Value { get; set; }

    }
}
