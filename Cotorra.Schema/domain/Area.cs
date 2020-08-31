using CotorraNode.Common.Base.Schema; 
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class Area : IdentityCatalogEntityExt
    {
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public virtual List<Department> Department   { get; set; }

    }
}
