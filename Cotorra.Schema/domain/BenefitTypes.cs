using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public enum BenefitTypeValue
    {
        Law = 1,
        Personalized = 2
    }
}
