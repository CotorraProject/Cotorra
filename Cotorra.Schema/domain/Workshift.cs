using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class Workshift : IdentityCatalogEntityExt
    { 
        [DataMember]
        public double Hours { get; set; }     
        [DataMember]
        public ShiftWorkingDayType ShiftWorkingDayType { get; set; }

    }
}
