using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CalculationFireAndForgetByEmployeeParams
    {
        [DataMember]
        public List<Guid> EmployeeIds { get; set; }

        [DataMember]
        public Guid IdentityWorkID { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }
    }
}
