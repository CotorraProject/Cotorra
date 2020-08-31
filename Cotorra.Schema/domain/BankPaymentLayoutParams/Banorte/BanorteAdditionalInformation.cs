using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class BanorteAdditionalInformation : IBankAdditionalInformation
    {
        public string SystemID { get; set; }
        public string ChargeAccount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string CompanyName { get; set; }
    }
}
