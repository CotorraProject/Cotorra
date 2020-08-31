using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public enum EmployeeTrustLevel
    {
        Unionized = 1,
        Trusted = 2
    }
}
