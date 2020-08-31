using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class Department : IdentityCatalogEntityExt
    {
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public string BanksBeneficiary { get; set; } 
        [DataMember]
        public Guid? AreaID { get; set; }
        [DataMember]
        public virtual Area Area { get; set; }

    }
}
