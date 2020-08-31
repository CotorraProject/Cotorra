using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public interface IConcept
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public ConceptType ConceptType { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public bool GlobalAutomatic { get; set; }

        [DataMember]
        public bool AutomaticDismissal { get; set; }

        [DataMember]
        public bool Kind { get; set; }

        [DataMember]
        public bool Print { get; set; }

        [DataMember]
        public string SATGroupCode { get; set; }

        [DataMember]
        public string Label { get; set; }

        [DataMember]
        public string Formula { get; set; }


        [DataMember]
        public string Label1 { get; set; }

        [DataMember]
        public string Label2 { get; set; }

        [DataMember]
        public string Label3 { get; set; }

        [DataMember]
        public string Label4 { get; set; }

        [DataMember]
        public string Formula1 { get; set; }

        [DataMember]
        public string Formula2 { get; set; }

        [DataMember]
        public string Formula3 { get; set; }

        [DataMember]
        public string Formula4 { get; set; }

        //[DataMember]
        //public List<AmmountConcept> AmmountConcepts { get; set; }
    }
}
