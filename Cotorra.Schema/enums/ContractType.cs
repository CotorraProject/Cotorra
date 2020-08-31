using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Cotorra.Schema
{
    public enum ContractType
    {
        /// <summary>
        /// 1. Contrato de trabajo por tiempo indeterminado
        /// </summary>
        [Display(Name = "1", Description = "Contrato de trabajo por tiempo indeterminado")]
        IndefiniteTermEmploymentContract = 1,

        /// <summary>
        /// 2. Contrato de trabajo por obra determinada
        /// </summary>
        [Display(Name = "2", Description = "Contrato de trabajo por obra determinada")]
        WorkContractForSpecificWork = 2,

        /// <summary>
        /// 3. Contrato de trabajo por tiempo determinado
        /// </summary>
        [Display(Name = "3", Description = "Contrato de trabajo por tiempo determinado")]
        FixedTermEmploymentContract = 3,

        /// <summary>
        /// 4. Contrato de trabajo por temporada
        /// </summary>
        [Display(Name = "4", Description = "Contrato de trabajo por temporada")]
        SeasonalEmploymentContract = 4,

        /// <summary>
        /// 5. Contrato de trabajo sujeto a prueba
        /// </summary>
        [Display(Name = "5", Description = "Contrato de trabajo sujeto a prueba")]
        TestEmploymentContract = 5,

        /// <summary>
        /// 6. Contrato de trabajo con capacitación inicial
        /// </summary>
        [Display(Name = "6", Description = "Contrato de trabajo con capacitación inicial")]
        EmploymentContractWithInitialTraining = 6,

        /// <summary>
        /// 7. Modalidad de contratación por pago de hora laborada
        /// </summary>
        [Display(Name = "7", Description = "Modalidad de contratación por pago de hora laborada")]
        ContractModalityForPaymentOfHoursWorked = 7,

        /// <summary>
        /// 8. Modalidad de trabajo por comisión laboral
        /// </summary>
        [Display(Name = "8", Description = "Modalidad de trabajo por comisión laboral")]
        ModalityOfWorkByLaborCommission = 8,

        /// <summary>
        /// 9. Modalidades de contratación donde no existe relación de trabajo
        /// </summary>
        [Display(Name = "9", Description = "Modalidades de contratación donde no existe relación de trabajo")]
        ContractModalitiesWhereThereIsNoEmploymentRelationship = 9,

        /// <summary>
        /// 10. Jubilación, Pensión y Retiro
        /// </summary>
        [Display(Name = "10", Description = "Jubilación, Pensión y Retiro")]
        RetirementPensionAndRetirement = 10,

        /// <summary>
        /// Otro contrato
        /// </summary>
        [Display(Name = "99", Description = "Otro contrato")]
        OtherContract = 99
    } 
}
