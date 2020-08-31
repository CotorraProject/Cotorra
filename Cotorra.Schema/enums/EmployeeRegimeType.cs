using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public enum EmployeeRegimeType
    {
        /// <summary>
        /// 2. Sueldos (incluye ingresos señalados en la fracción I del artículo 94 de LISR)
        /// </summary>
        Salaries = 2,

        /// <summary>
        /// 3. Jubilados
        /// </summary>
        Retired = 3,

        /// <summary>
        /// 4. Pensionados
        /// </summary>
        Pensioners = 4,

        /// <summary>
        /// 5. Asimilados miembros sociedades cooperativas producción
        /// </summary>
        AssimilatedMembersCooperativeSocietiesProduction = 5,

        /// <summary>
        /// 6. Asimilados integrantes sociedades asociaciones civiles
        /// </summary>
        AssimilatedMembersOfCivilAssociationSocieties = 6,

        /// <summary>
        /// 7. Asimilados miembros consejos
        /// </summary>
        AssimilatedCouncilMembers = 7,

        /// <summary>
        /// 8. Asimilados comisionistas
        /// </summary>
        AssimilatedCommissionAgents = 8,

        /// <summary>
        /// 9. Asimilados honorarios
        /// </summary>
        AssimilatedFee = 9,

        /// <summary>
        /// 10. Asimilados acciones
        /// </summary>
        AssimilatedActions = 10,

        /// <summary>
        /// 11. Asimilados otros
        /// </summary>
        AssimilatedOthers = 11,

        /// <summary>
        /// 12. Jubilados o Pensionados
        /// </summary>
        RetiredPensioners = 12,

        /// <summary>
        /// Indemnización o Separación
        /// </summary>
        Compensation = 13,

        /// <summary>
        /// Otro régimen
        /// </summary>
        Other = 99, 
    }
}
