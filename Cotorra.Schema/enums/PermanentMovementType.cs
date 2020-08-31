using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum PermanentMovementType
    {
        Amount = 1,
        Value = 2
    }
}
