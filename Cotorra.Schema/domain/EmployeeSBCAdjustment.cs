﻿using System;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class EmployeeSBCAdjustment : IdentityCatalogEntityExt
    {
        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public DateTime ModificationDate { get; set; }

        [DataMember]
        public decimal SBCFixedPart { get; set; }

        [DataMember]
        public decimal SBCVariablePart { get; set; }

        [DataMember]
        public decimal SBCMax25UMA { get; set; }
    }
}
