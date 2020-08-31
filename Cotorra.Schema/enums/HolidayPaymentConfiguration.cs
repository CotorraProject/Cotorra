using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Base de cotización
    /// </summary>
    public enum HolidayPaymentConfiguration
    {
        //Pagar vacaciones y prima vacacional en el periodo que abarquen
        PayVacationsAndBonusInPeriod = 0,
        //Pagar solo vacaciones en el periodo que abarquen
        PayVacationInPeriod = 1,
        //Pagar solo vacaciones en el periodo que comienzan
        PayVacationInitially = 2,

 
    }
}
