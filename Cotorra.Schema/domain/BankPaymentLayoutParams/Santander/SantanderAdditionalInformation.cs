using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class SantanderAdditionalInformation : IBankAdditionalInformation
    {
        public DateTime GenerationDate { get; set; }
        public string CompanyBankAccount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
