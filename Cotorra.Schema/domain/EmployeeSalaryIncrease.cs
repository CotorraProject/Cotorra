using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class EmployeeSalaryIncrease : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        [DataMember]
        public decimal DailySalary { get; set; }

        [DataMember]
        public Guid EmployeeSBCAdjustmentID { get; set; }

        [DataMember]
        public virtual EmployeeSBCAdjustment EmployeeSBCAdjustment { get; set; }
    }
}
