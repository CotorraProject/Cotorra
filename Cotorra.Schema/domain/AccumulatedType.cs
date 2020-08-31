using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum TypeOfAccumulated
    {
        SalaryPaymentDeductionLiability = 1,
        DaysHours = 2
    }

    /// <summary>
    /// Tipos de acumulados
    /// </summary>
    [DataContract]
    [Serializable]
    public class AccumulatedType : IdentityCatalogEntityExt
    {
        [DataMember]
        public int? Code { get; set; }

        [DataMember]
        public TypeOfAccumulated TypeOfAccumulated { get; set; }
    }
}
