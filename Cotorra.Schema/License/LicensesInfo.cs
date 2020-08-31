using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class LicenseInfoP
    {
        public LicenseInfoP() { }

        public Guid LicenseID { get; set; }
        public string Code { get; set; }
        public Guid OwnerID { get; set; }
        public string OwnerFirstName { get; set; }
        public string OwnerLastName { get; set; }
        public string OwnerSecondLastName { get; set; }
        public string OwnerEmail { get; set; }
        public bool IsOwner { get; set; }
        public List<LicenseApps> Apps { get; set; }
    }

    public class LicenseApps
    {
        public LicenseApps() { }

        public Guid AppID { get; set; }
        public string AppName { get; set; }
        public Guid ServiceID { get; set; }
        public string ServiceName { get; set; }
        public Guid LicenseServiceID { get; set; }
        public bool HasManagerRole { get; set; }
        public bool IsExpired { get; }
        public bool IsMainAppService { get; }
        public DateTime? ExpirationDate { get; set; }
        public int StatusID { get; set; }
        public DateTime ActivationDate { get; set; }
        public List<LicensingFeatures> Features { get; set; }
    }

    public class LicensingFeatures
    {
        public LicensingFeatures() { }

        public Guid ID { get; set; }
        public string Name { get; set; }
        public decimal Limit { get; set; }
        public decimal Applied { get; set; }
    }

}
