using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{
    public interface ICalculateParams
    {
        [DataMember]
        public Guid InstanceID { get; set; }
    }
}
