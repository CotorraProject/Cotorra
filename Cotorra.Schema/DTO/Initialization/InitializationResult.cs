using CotorraNode.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class InitializationResult
    {
     

        [DataMember]
        public Guid  InstanceID { get; set; }

        [DataMember]
        public Guid CompanyID { get; set; }

        [DataMember]
        public Guid LicenseID { get; set; }

        [DataMember]
        public Guid LicenseServiceID { get; set; }
 
    }
}
