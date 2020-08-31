using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum RiskType
    {
        WorkAccident = 0,
        JourneyAccident = 1,
        ProfessionalDesease = 2
    }
}
