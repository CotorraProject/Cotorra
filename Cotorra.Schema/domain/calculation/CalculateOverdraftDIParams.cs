using CotorraNode.Common.Base.Schema.Parameters.Base;
using Cotorra.Schema.Calculation;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class CalculateOverdraftDIParams : IdentityWorkParams, ICalculateParams
    {
        public CalculateOverdraftDIParams()
        {
            OverdraftDI = new OverdraftDI();
        }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public bool ResetCalculation { get; set; }

        [DataMember]
        public OverdraftDI OverdraftDI { get; set; }


        [DataMember]
        public string Formula { get; set; }
    }

    [DataContract]
    public class OverdraftDI
    {
        public OverdraftDI()
        {
            ID = Guid.NewGuid();
            OverdraftDetailDIs = new List<OverdraftDetailDI>();
            PeriodDetailDI = new PeriodDetailDI();
            EmployeeDI = new EmployeeDI();
        }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public OverdraftType OverdraftType { get; set; }       

        [DataMember]
        public OverdraftStatus OverdraftStatus { get; set; }

        [DataMember]
        public PeriodDetailDI PeriodDetailDI { get; set; }

        [DataMember]
        public EmployeeDI EmployeeDI { get; set; }

        [DataMember]
        public List<OverdraftDetailDI> OverdraftDetailDIs { get; set; }
    }

    [DataContract]
    public class EmployeeDI
    {
        public EmployeeDI()
        {
            ID = Guid.NewGuid();
            WorkshiftDI = new WorkshiftDI();
        }

        [DataMember]
        public Guid ID { get; set; }      

        [DataMember]
        public string FullName { get; set; }

        [DataMember]
        public DateTime EntryDate { get; set; }

        [DataMember]
        public decimal DailySalary { get; set; }

        [DataMember]
        public BaseQuotation ContributionBase { get; set; }

        [DataMember]
        public decimal SBCFixedPart { get; set; }

        [DataMember]
        public decimal SBCVariablePart { get; set; }

        [DataMember]
        public decimal SettlementSalaryBase { get; set; }

        [DataMember]
        public decimal SBCMax25UMA { get; set; }

        [DataMember]
        public SalaryZone SalaryZone { get; set; }

        [DataMember]
        public WorkshiftDI WorkshiftDI { get; set; }
    }

    [DataContract]
    public class WorkshiftDI
    {
        [DataMember]
        public double Hours { get; set; }     
    }

    [DataContract]
    public class PeriodDetailDI
    {
        public PeriodDetailDI()
        {
            ID = Guid.NewGuid();
            PeriodDI = new PeriodDI();
        }

        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public DateTime InitialDate { get; set; }

        [DataMember]
        public DateTime FinalDate { get; set; }

        [DataMember]
        public PeriodMonth PeriodMonth { get; set; }

        [DataMember]
        public PeriodBimonthlyIMSS PeriodBimonthlyIMSS { get; set; }

        [DataMember]
        public PeriodFiscalYear PeriodFiscalYear { get; set; }

        [DataMember]
        public decimal PaymentDays { get; set; }        

        [DataMember]
        public string SeventhDayPosition { get; set; }

        [DataMember]
        public int SeventhDays { get; set; }

        [DataMember]
        public PeriodDI PeriodDI { get; set; }
    }

    public class PeriodDI
    {
        [DataMember]
        public Int32 FiscalYear { get; set; }

        /// <summary>
        /// Días pagados en quincena
        /// </summary>
        [DataMember]
        public AdjustmentPay_16Days_Febrary FortnightPaymentDays { get; set; }
    }

    [DataContract]
    public class OverdraftDetailDI
    {
        public OverdraftDetailDI()
        {
            ConceptPaymentDI = new ConceptPaymentDI();
        }

        [DataMember]
        public Guid ID { get; set; }

        /// <summary>
        /// Valor, Días, "Leyenda del valor" en conceptos
        /// </summary>
        [DataMember]
        public decimal Value { get; set; }

        /// <summary>
        /// Total
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        [DataMember]
        public decimal Taxed { get; set; }

        [DataMember]
        public decimal Exempt { get; set; }

        [DataMember]
        public decimal IMSSTaxed { get; set; }

        [DataMember]
        public decimal IMSSExempt { get; set; }

        [DataMember]
        public bool IsGeneratedByPermanentMovement { get; set; }

        [DataMember]
        public bool IsValueCapturedByUser { get; set; }

        [DataMember]
        public bool IsTotalAmountCapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount1CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount2CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount3CapturedByUser { get; set; }

        [DataMember]
        public bool IsAmount4CapturedByUser { get; set; }


        [DataMember]
        public ConceptPaymentDI ConceptPaymentDI { get; set; }
    }

    [DataContract]
    public class ConceptPaymentDI
    {
        [DataMember]
        public Guid ID { get; set; }

        [DataMember]
        public int Code { get; set; }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public ConceptType ConceptType { get; set; }      
        
        [DataMember]
        public string FormulaTotal { get; set; }

        [DataMember]
        public string FormulaValue { get; set; }

        [DataMember]
        public string FormulaTaxed { get; set; }

        [DataMember]
        public string FormulaExempt { get; set; }

        [DataMember]
        public string FormulaIMSSTaxed { get; set; }

        [DataMember]
        public string FormulaIMSSExempt { get; set; }
    }
}
