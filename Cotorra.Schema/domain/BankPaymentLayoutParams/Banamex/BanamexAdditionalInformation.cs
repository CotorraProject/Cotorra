using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class BanamexAdditionalInformation : IBankAdditionalInformation
    {
        public string CustomerNumber { get; set; }
        public DateTime PaymentDate { get; set; }
        public string FileNumberOfDay { get; set; }
        public string BranchOfficeNumber { get; set; }
        public string ChargeAccount { get; set; }
        public string StateID { get; set; }
        public string CityID { get; set; }
        public string CompanyName { get; set; }
    }
}
