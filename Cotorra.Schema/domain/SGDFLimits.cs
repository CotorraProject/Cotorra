using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    public class SGDFLimits : IdentityCatalogEntityExt
    {
        [DataMember]
        public DateTime ValidityDate { get; set; }

        [DataMember]
        public decimal EG_Especie_GastosMedicos_1 { get; set; }

        [DataMember]
        public decimal EG_Especie_Fija_2 { get; set; }

        [DataMember]
        public decimal EG_Especie_mas_3SMDF_3 { get; set; }

        [DataMember]
        public decimal EG_Prestaciones_en_Dinero_4 { get; set; }

        [DataMember]
        public decimal Invalidez_y_vida_5 { get; set; }

        [DataMember]
        public decimal Cesantia_y_vejez_6 { get; set; }

        [DataMember]
        public decimal Guarderias_7 { get; set; }

        [DataMember]
        public decimal Retiro_8 { get; set; }

        [DataMember]
        public decimal RiesgodeTrabajo_9 { get; set; }
    }
}
