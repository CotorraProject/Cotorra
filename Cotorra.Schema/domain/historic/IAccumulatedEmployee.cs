using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    public interface IAccumulatedEmployee
    {
        [DataMember]
        public Guid EmployeeID { get; set; }

        [DataMember]
        public Guid AccumulatedTypeID { get; set; }

        [DataMember]
        public int ExerciseFiscalYear { get; set; }

        [DataMember]
        public decimal InitialExerciseAmount { get; set; }

        [DataMember]
        public decimal PreviousExerciseAccumulated { get; set; }

        [DataMember]
        public decimal January { get; set; }

        [DataMember]
        public decimal February { get; set; }

        [DataMember]
        public decimal March { get; set; }

        [DataMember]
        public decimal April { get; set; }

        [DataMember]
        public decimal May { get; set; }

        [DataMember]
        public decimal June { get; set; }

        [DataMember]
        public decimal July { get; set; }

        [DataMember]
        public decimal August { get; set; }

        [DataMember]
        public decimal September { get; set; }

        [DataMember]
        public decimal October { get; set; }

        [DataMember]
        public decimal November { get; set; }

        [DataMember]
        public decimal December { get; set; }

        //Virtuals
        [DataMember]
        public AccumulatedType AccumulatedType { get; set; }

        [DataMember]
        public Employee Employee { get; set; }
    }
}
