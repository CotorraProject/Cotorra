using CotorraNode.Common.Base.Schema;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema
{
    [DataContract]
    [Serializable]
    public class catCFDI_CodigoPostal : CatalogEntity
    {
        [DataMember]
        public string c_CodigoPostal { get; set; }

        [DataMember]
        public string c_Estado { get; set; }

        [DataMember]
        public string c_Municipio { get; set; }

        [DataMember]
        public string c_Localidad { get; set; }

        [DataMember]
        public string Estimulo_Franja_Fronteriza { get; set; }

        [DataMember]
        public string FechaInicioVigencia { get; set; }

        [DataMember]
        public string FechaFinVigencia { get; set; }

        [DataMember]
        public string DescripcionHusoHorario { get; set; }

        [DataMember]
        public string Mes_Inicio_Horario_Verano { get; set; }
        
        [DataMember]
        public string Dia_Inicio_Horario_Verano { get; set; }

        [DataMember]
        public string Hora_Horario_Verano { get; set; }

        [DataMember]
        public string Diferencia_Horaria_Verano  { get; set; }

        [DataMember]
        public string Mes_Inicio_Horario_Invierno { get; set; }

        [DataMember]
        public string Dia_Inicio_Horario_Invierno { get; set; }

        [DataMember]
        public string Hora_Inicio_Horario_Invierno { get; set; }

        [DataMember]
        public string Diferencia_Horaria_Invierno { get; set; }

    }
}