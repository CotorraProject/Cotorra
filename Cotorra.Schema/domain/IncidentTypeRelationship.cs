using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class IncidentTypeRelationship : IdentityCatalogEntityExt
    {
        [IgnoreDataMember]

        public Guid IncidentTypeID { get; set; }

        [DataMember]
        public Guid AccumulatedTypeID { get; set; }      

        [DataMember]
        public virtual IncidentType IncidentType { get; set; }

        [DataMember]
        public virtual AccumulatedType AccumulatedType { get; set; }
    }
}
