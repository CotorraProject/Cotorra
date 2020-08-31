using CotorraNode.Common.Base.Schema;
using Cotorra.Schema.domain.nom035;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationSurvey : CatalogEntity
    {
        [DataMember]
        public Guid NOMEvaluationGuideID { get; set; }

        [DataMember]
        public int Number { get; set; }

        [DataMember]
        public virtual NOMEvaluationGuide NOMEvaluationGuide { get; set; }

        [DataMember]
        [JsonIgnore]
        public virtual List<NOMEvaluationPhase> NOMEvaluationPhases { get; set; }

    }
}
