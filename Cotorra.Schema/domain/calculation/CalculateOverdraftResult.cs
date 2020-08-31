using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{
    [DataContract]
    public class CalculateOverdraftResult : ICalculateResult
    {
        [DataMember]
        public Overdraft OverdraftResult { get; set; }

    }
}
