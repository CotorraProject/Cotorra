using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum PermanentMovementStatus
    {
        Active = 1,
        Inactive = 2
    }
}
