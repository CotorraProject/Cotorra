using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class EmployerFiscalInformation : IdentityCatalogEntityExt
    {
        [DataMember]
        public string RFC { get; set; }

        [DataMember]
        public string CertificateCER { get; set; }

        [DataMember]
        public string CertificateKEY { get; set; }

        [DataMember]
        public string CertificatePwd { get; set; }

        [DataMember]
        public DateTime StartDate { get; set; }

        [DataMember]
        public DateTime EndDate { get; set; }

        [DataMember]
        public string CertificateNumber { get; set; }
    }
}
