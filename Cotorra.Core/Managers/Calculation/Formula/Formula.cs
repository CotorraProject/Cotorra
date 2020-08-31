using System;
using System.Runtime.Serialization;

namespace Cotorra.Core.Managers.Calculation
{

    public enum CategoryFormula
    {
        NoSpecified = 0,
        WithoutCategory = 1,
        GeneralsIMSS = 3,
        Vacations = 4,
        EG1Calculation = 5,
        EG2Calculation = 6,
        EG3Calculation = 7,
        EG4Calculation = 8,
        Table5Calculation = 9,
        Table6Calculation = 10,
        Table7Calculation = 11,
        Table8Calculation = 12,
        //IMSSWorkRisk
        IMSSWorkRisk = 13,

        //ISPT_Art142
        ISPT_Art142 = 14,

        //ISR_Settlement
        ISR_Settlement = 15,

        //ISPT GENERALES 2010
        ISPT_Generals = 16,

        //ISPT_Mensual sp 2010
        ISPT_Monthly = 17,

        //ISPT Mensualizada 2010
        ISPT_Monthlylized = 18,

        //Préstamos y Créditos
        LoanesCredits = 19,

        //Ajuste Mes Subsidio
        MonthFixedSubsidy = 20,

        //INFONAVIT
        INFONAVIT = 21,

        //Misc
        Misc = 22,

        //Perceptions
        Perceptions = 23,

        //Deductions
        Deductions = 24,

        //Tiempo extra
        ExtraHours = 25
    }

    [DataContract]
    public class Formula
    {
        [DataMember]
        public CategoryFormula CategoryFormula { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Content { get; set; }

        [DataMember]
        public Type FunctionExtensionType { get; set; }
    }
}
