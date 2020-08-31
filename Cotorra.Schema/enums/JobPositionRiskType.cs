using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum JobPositionRiskType
    {
        Class_I = 1,
        Class_II = 2,
        Class_III = 3,
        Class_IV = 4,
        Class_V = 5,
        NA = 99
    }
}
