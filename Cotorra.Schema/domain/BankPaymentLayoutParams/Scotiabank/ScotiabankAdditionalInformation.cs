using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class ScotiabankAdditionalInformation : IBankAdditionalInformation
    {
        public string CustomerNumber { get; set; }
        public string FileNumberOfDay { get; set; }
        public string ChargeAccount { get; set; }
        public string CompanyReference { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
