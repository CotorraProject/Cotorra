using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{ 
    public class BankLayoutPaymentInformation
    {
        public string BankCode { get; set; }
        public string BankName { get; set; }
        public Guid PeriodDetailID { get; set; }
        public string PeriodDescription { get; set; }
        public Guid InstanceID { get; set; }
        public Guid IdentityWorkID { get; set; }
        public string TokeUser { get; set; }
        public bool IncludeInactiveEmployees { get; set; }

        public BankExtraParams BankExtraParams { get; set; }
    }

    public class BankExtraParams
    {

        public BanamexAdditionalInformation Banamex { get; set; }
        public ScotiabankAdditionalInformation Scotiabank { get; set; }
        public SantanderAdditionalInformation Santander { get; set; }
        public BanorteAdditionalInformation Banorte { get; set; }
    }

}
