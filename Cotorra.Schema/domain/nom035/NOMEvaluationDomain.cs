using CotorraNode.Common.Base.Schema;
using Cotorra.Schema.nom035;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationDomain : CatalogEntity
    {
        [DataMember]
        public int Number { get; set; }

        [DataMember]
        [JsonIgnore]
        public virtual List<NOMEvaluationQuestion> NOMEvaluationQuestions { get; set; }
    }
}

