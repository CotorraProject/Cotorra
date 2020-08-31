using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum ShiftWorkingDayType
    {
        /// <summary>
        /// Diurna
        /// </summary>
        Daytime = 1,
        /// <summary>
        /// Nocturna
        /// </summary>
        Night = 2,
        /// <summary>
        /// Mixta
        /// </summary>
        Mixed = 3,
        /// <summary>
        /// Por horas
        /// </summary>
        PerHours = 4,
        /// <summary>
        /// Reducida
        /// </summary>
        Reduced = 5,
        /// <summary>
        /// Continuada
        /// </summary>
        Continued = 6,
        /// <summary>
        /// Partida
        /// </summary>
        Entry = 7,
        /// <summary>
        /// Por turnos
        /// </summary>
        ByTurns = 8,
        /// <summary>
        /// Otra jornada
        /// </summary>
        Other = 99
    }
}
