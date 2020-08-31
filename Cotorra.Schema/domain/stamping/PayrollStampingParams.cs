using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    /// <summary>
    /// Fundamento Legal: Artículo 3-B de la Ley de Coordinación Fiscal. 
    /// </summary>
    [DataContract]
    public enum SNCFOriginResourceType
    {
        /// <summary>
        /// Ingresos propios: Son los ingresos pagados por Entidades  federativas, municipios o demarcaciones territoriales 
        /// del Distrito Federal, organismos autónomos y entidades paraestatales y paramunicipales con cargo a sus participaciones 
        /// u otros ingresos locales. 
        /// </summary>
        IP = 1,

        /// <summary>
        /// Ingresos federales: Son los ingresos pagados por Entidades  federativas, municipios o demarcaciones territoriales 
        /// del Distrito Federal, organismos autónomos y entidades paraestatales y paramunicipales con recursos federales, 
        /// distintos a las participaciones. 
        /// </summary>
        IF = 2,

        /// <summary>
        /// Ingresos mixtos: Son los ingresos pagados por Entidades federativas, municipios o demarcaciones territoriales 
        /// del Distrito Federal, organismos autónomos y entidades paraestatales y paramunicipales con cargo a sus participaciones 
        /// u otros ingresos locales y con recursos federales distintos a las participaciones. 
        /// </summary>
        IM = 3,
    }

    /// <summary>
    /// Mexican government
    /// Este nodo sólo aplica para las entidades federativas, municipios, así como sus respectivos organismos autónomos y entidades paraestatales y paramunicipales.  
    /// El proveedor de certificación validará que en los sistemas del SAT exista clave en el RFC del emisor como RFC inscrito y no cancelado.En caso contrario este campo no debe existir.
    /// </summary>
    [DataContract]
    public class SNCFEntity
    {
        public SNCFOriginResourceType SNCFOriginResourceType { get; set; }

        /// <summary>
        /// Cuando se señale que el origen del recurso es por ingresos mixtos, se debe registrar únicamente el importe bruto 
        /// de los ingresos propios, incluyendo el total de ingresos gravados y exentos. 
        ///El valor de este dato debe ser menor que la suma de los campos TotalPercepciones y TotalOtrosPagos.
        /// </summary>
        public decimal AmountOwnResource { get; set; }
    }

    [DataContract]
    public class PayrollStampingDetail 
    {
        [DataMember]
        public Guid OverdraftID { get; set; }

        [DataMember]
        public string Series { get; set; }

        [DataMember]
        public string Folio { get; set; }

        [DataMember]
        public DateTime PaymentDate { get; set; }

        [DataMember]
        public string RFCOriginEmployer { get; set; }

        [DataMember]
        public SNCFEntity SNCFEntity { get; set; }
               
    }

    [DataContract]
    public class PayrollStampingParams : IdentityWorkParams
    {
        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid user { get; set; }

        [DataMember]
        public Guid PeriodDetailID { get; set; }

        [DataMember]
        public FiscalStampingVersion FiscalStampingVersion { get; set; }

        [DataMember]
        public Currency Currency { get; set; }

        [DataMember]
        public List<PayrollStampingDetail> Detail { get; set; }

      
    }
}
