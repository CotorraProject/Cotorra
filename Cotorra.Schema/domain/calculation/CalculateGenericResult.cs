using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{ 
    [DataContract]
    public class CalculateGenericResult : ICalculateResult
    {
        [DataMember]
        public decimal Result { get; set; }

        [DataMember]
        public string ResultText { get; set; }

        [DataMember]
        public List<CalculateArgument> CalculateArguments { get; set; }
    }
}
