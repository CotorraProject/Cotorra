using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum InhabilityControl
    {
        None = 0,
        Unique = 1,
        Initial = 2,
        Subsequent = 3,
        MedicalCheckoutST2 = 4,
        ValuationST3 = 5,
        DeathST3 = 6,
        Prenatal = 7,
        Link = 8,
        Postnatal = 9
    }
}
