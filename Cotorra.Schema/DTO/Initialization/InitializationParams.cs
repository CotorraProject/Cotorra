using CotorraNode.CommonApp.Schema;
using CotorraNube.CommonApp.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class InitializationParams
    {
        [DataMember]
        public string AuthTkn { get; set; }

        [DataMember]
        public Guid LicenseServiceID { get; set; }

        [DataMember]
        public string SocialReason { get; set; }

        [DataMember]
        public string RFC { get; set; }

        [DataMember]
        public string Alias { get; set; }

        [DataMember]

        public PayrollCompanyConfiguration PayrollCompanyConfiguration { get; set; }

        [DataMember]
        public EmployerRegistration EmployerRegistration { get; set; }

    }
}
