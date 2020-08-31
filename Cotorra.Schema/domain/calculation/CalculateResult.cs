using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class CalculateArgument
    {
        [DataMember]
        public string Argument { get; set; }

        [DataMember]
        public string Expression { get; set; }

        [DataMember]
        public decimal Value { get; set; }
    }

    [DataContract]
    public class CalculateResult
    {
        public CalculateResult()
        {
            CalculateArguments = new List<CalculateArgument>();
        }

        [DataMember]
        public decimal Result { get; set; }

        [DataMember]
        public string ResultText { get; set; }

        [DataMember]
        public List<CalculateArgument> CalculateArguments { get; set; }
    }
}
