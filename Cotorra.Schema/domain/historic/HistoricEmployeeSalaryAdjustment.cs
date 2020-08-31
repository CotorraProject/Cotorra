using CotorraNode.Common.Base.Schema;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class HistoricEmployeeSalaryAdjustment : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        [DataMember]
        public decimal DailySalary { get; set; }     

    }
}
