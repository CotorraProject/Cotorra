using CotorraNode.Common.Base.Schema;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationPhase : CatalogEntity
    {
        [DataMember]
        public Guid NOMEvaluationSurveyID { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public virtual NOMEvaluationSurvey NOMEvaluationSurvey { get; set; }

        [DataMember]
        [JsonIgnore]
        public virtual List<NOMEvaluationQuestion> NOMEvaluationQuestions { get; set; }
    }
}
