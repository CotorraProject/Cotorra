using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public enum VacationsCaptureType
    {
        PayVacationsAndBonusInPeriod = 0,
        //Pagar solo vacaciones en el periodo que abarquen
        PayVacationInPeriod = 1,
        //Pagar solo vacaciones en el periodo que comienzan
        PayVacationInitially = 2,
    }
}
