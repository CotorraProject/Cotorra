using CotorraNode.Common.Base.Schema;
using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class EmployeeIdentityRegistration : CatalogEntity
    {
        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public Guid? IdentityUserID { get; set; }

        [DataMember]
        public string Email { get; set; }

        [DataMember]
        public string ActivationCode { get; set; }

        [DataMember]
        public EmployeeIdentityRegistrationStatus EmployeeIdentityRegistrationStatus { get; set; }
    }
}
