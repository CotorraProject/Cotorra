using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class FunctionParams
    {
        public FunctionParams()
        {
            EmployeeConceptsRelationDetails = new List<EmployeeConceptsRelationDetail>();
        }

        [DataMember]
        public Guid IdentityWorkID { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public CalculationBaseResult CalculationBaseResult { get; set; }

        [DataMember]
        public List<EmployeeConceptsRelationDetail> EmployeeConceptsRelationDetails { get; set; }
    }
}
