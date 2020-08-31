using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    public enum SalaryZone
    {
        [Display(Name = "1", Description = "Zona A")]
        ZoneA = 1,//  Zona A,

        [Display(Name = "2", Description = "Zona B")]
        ZoneB = 2,//   Zona B

        [Display(Name = "3", Description = "Zona C")]
        ZoneC = 3,//  Zona C 
    }
}