using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class Incident : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid IncidentTypeID { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public decimal Value { get; set; }

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public virtual IncidentType IncidentType { get; set; }

        //[DataMember]
        //public virtual Employee Employee { get; set; }
    }
}
