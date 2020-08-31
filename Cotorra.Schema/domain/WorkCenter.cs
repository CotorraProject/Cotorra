using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class WorkCenter : IdentityCatalogEntityExt
    {
        [DataMember]
        public int Key { get; set; }
        [DataMember]
        public string Observations { get; set; }
        [DataMember]
        public String ZipCode { get; set; }
        [DataMember]
        public String FederalEntity { get; set; }
        [DataMember]
        public String Municipality { get; set; }
        [DataMember]
        public String Street { get; set; }
        [DataMember]
        public String ExteriorNumber { get; set; }
        [DataMember]
        public String InteriorNumber { get; set; }
        [DataMember]
        public String Suburb { get; set; }
        [DataMember]
        public String Reference { get; set; }

    }
}
