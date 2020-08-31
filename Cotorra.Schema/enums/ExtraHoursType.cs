using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    /// <summary>
    /// Tipo de Horas Extra
    /// </summary>
    [DataContract]
    public enum ExtraHoursType
    {
        Double = 01,
        Triple = 02,
        Simple = 03
    }
}
