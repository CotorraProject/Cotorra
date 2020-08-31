using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class SetStatusParams
    {
        [DataMember]
        public Guid IdentityWorkID { get; set;}

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public List<Guid> IDS { get; set; }

        [DataMember]
        public CotorriaStatus Status { get; set; }

        [DataMember]

        public string TypeFullName { get; set; }
    }
}
