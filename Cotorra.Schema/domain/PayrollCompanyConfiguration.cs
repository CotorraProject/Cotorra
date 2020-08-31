using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    [Table("PayrollCompanyConfiguration")]
    public class PayrollCompanyConfiguration : IdentityCatalogEntityExt
    {
        /// <summary>
        /// RFC
        /// </summary>
        [DataMember]
        public String RFC { get; set; }

        /// <summary>
        /// Razón Social
        /// </summary>
        [DataMember]
        public String SocialReason { get; set; }

        /// <summary>
        /// Para personas físicas
        /// </summary>
        [DataMember]
        public String CURP { get; set; }

        /// <summary>
        /// Zona de salario general
        /// </summary>
        [DataMember]
        public SalaryZone SalaryZone { get; set; }

        /// <summary>
        /// Régimen Fiscal
        /// </summary>
        [DataMember]
        public FiscalRegime FiscalRegime { get; set; }

        /// <summary>
        /// Factor no deducible por ingresos excentos
        /// </summary>
        [DataMember]
        public decimal NonDeducibleFactor { get; set; }

        /// <summary>
        /// Ejercicio vigente
        /// </summary>
        [DataMember]
        public int CurrentExerciseYear { get; set; }

        /// <summary>
        /// Fecha de inicio de historia
        /// </summary>
        [DataMember]
        public DateTime StartDate { get; set; }

        /// <summary>
        /// Moneda
        /// </summary>
        [DataMember]
        public Guid CurrencyID { get; set; }

        /// <summary>
        /// Dirección
        /// </summary>
        public Guid? AddressID { get; set; }

        /// <summary>
        /// Periodo actual
        /// </summary>
        [DataMember]
        public int CurrentPeriod { get; set; }

        /*DATOS ADICIONALES DE LA COMPAÑIA NO OBLIGATORIOS*/
        /// <summary>
        /// Pkaticanos sobre tu empresa, información libre.
        /// </summary>
        [DataMember]
        public string CompanyInformation { get; set; }

        /// <summary>
        /// Nombre comercial
        /// </summary>
        [DataMember]
        public string ComercialName { get; set; }

        /// <summary>
        /// Fecha de fundación
        /// </summary>
        [DataMember]
        public DateTime? CompanyCreationDate { get; set; }

        /// <summary>
        /// Tu empresa tiene ámbito
        /// </summary>
        [DataMember]
        public CompanyScope CompanyScope { get; set; }

        /// <summary>
        /// Email de la empresa
        /// </summary>
        [DataMember]
        public string CompanyContactEmail { get; set; }

        /// <summary>
        /// Teléfono de contacto de la empresa
        /// </summary>
        [DataMember]
        public string CompanyContactPhone { get; set; }

        /// <summary>
        /// Teléfono de contacto de la empresa
        /// </summary>
        [DataMember]
        public CompanyBusinessSector CompanyBusinessSector { get; set; }

        /// <summary>
        /// Company WebSite
        /// </summary>
        [DataMember]
        public string CompanyWebSite { get; set; }

        /// <summary>
        /// Facebook
        /// </summary>
        [DataMember]
        public string Facebook { get; set; }

        /// <summary>
        /// Instagram
        /// </summary>
        [DataMember]
        public string Instagram { get; set; }

        /// <summary>
        /// Twitter
        /// </summary>
        [DataMember]
        public string Twitter { get; set; }

        /// <summary>
        /// Youtube
        /// </summary>
        [DataMember]
        public string Youtube { get; set; }

        /*FIN DE DATOS ADICIONALES*/

        /// <summary>
        /// Virtual - Dirección
        /// </summary>
        public virtual Address Address { get; set; }

        #region "Necesarios para la creación de empresa pero no tienen persistencia"

        /// <summary>
        /// Dia de la semana para el séptimo
        /// </summary>
        [DataMember]
        [NotMapped]
        public WeeklySeventhDay WeeklySeventhDay { get; set; }

        /// <summary>
        /// Periodicidad de Pago (No está mapeado a BD)
        /// </summary>
        [DataMember]
        [NotMapped]
        public PaymentPeriodicity PaymentPeriodicity { get; set; }

        /// <summary>
        /// Fecha Inicial del Periodo (No está mapeado a BD)
        /// </summary>
        [DataMember]
        [NotMapped]
        public DateTime PeriodInitialDate { get; set; }

        /// <summary>
        /// Días de Pago (No está mapeado a BD)
        /// </summary>
        [DataMember]
        [NotMapped]
        public decimal PaymentDays { get; set; }

        /// <summary>
        /// Ajuste de pago (No está mapeado a BD)
        /// </summary>
        [DataMember]
        [NotMapped]
        public AdjustmentPay_16Days_Febrary AdjustmentPay { get; set; }
        #endregion
    }
}
