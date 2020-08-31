using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.thridparty
{
    [DataContract]
    public class IdentityPlatform
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public string Email { get; set; }
    }
}
