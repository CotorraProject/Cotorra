using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class ClientParams
    {
        [DataMember]
        public Guid IdentityWorkID { get; set;}

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public string FullNameType { get; set; }

        [DataMember]
        public string JsonListObjects { get; set; }

        [DataMember]
        public Guid LicenseID { get; set; }
        [DataMember]
        public string AuthTkn { get; set; }
        public Guid CotorriaAppID { get; set; }
        public Guid LicenseServiceID { get; set; }
    }
}
