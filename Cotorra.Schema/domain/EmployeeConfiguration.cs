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
    [Table("EmployeeConfiguration")]
    public class EmployeeConfiguration : IdentityCatalogEntityExt
    {
        /// <summary>
        /// Mascarilla del código del empleado
        /// </summary>
        [DataMember]
        public string EmployeeCodeFormat { get; set; }

        /// <summary>
        /// Tipo de código del empleado
        /// </summary>
        [DataMember]
        public CodeTypeEmployee CodeTypeEmployee { get; set; }
    }
}
