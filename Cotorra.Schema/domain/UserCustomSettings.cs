using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class UserCustomSettings : IdentityCatalogEntityExt
    {
        [DataMember]
        public string Key { get; set; }

        [DataMember]
        public string Value { get; set; }
    }
}
