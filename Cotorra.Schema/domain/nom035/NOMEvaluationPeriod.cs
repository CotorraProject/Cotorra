using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationPeriod : IdentityCatalogEntityExt
    {
        [DataMember]
        public string Period { get; set; }

        [DataMember]
        public bool State { get; set; }

        [DataMember]
        public DateTime InitialDate { get; set; } 

        [DataMember]
        public DateTime FinalDate { get; set; } 
    }
}
