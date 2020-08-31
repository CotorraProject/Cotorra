using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.domain.nom035
{
    [DataContract]
    [Serializable]
    public class NOMEvaluationGuide : CatalogEntity
    {
        [DataMember]
        public int Number { get; set; }

    }
}
