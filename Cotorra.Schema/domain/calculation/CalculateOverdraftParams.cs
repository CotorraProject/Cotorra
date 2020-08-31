using CotorraNode.Common.Base.Schema.Parameters.Base;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Cotorra.Schema.Calculation
{
    [DataContract]
    public class CalculateOverdraftParams : IdentityWorkParams, ICalculateParams
    {
        public CalculateOverdraftParams()
        {
            DeleteAccumulates = true;
            SaveOverdraft = true;
        }

        [DataMember]
        public Guid InstanceID { get; set; }

        [DataMember]
        public Guid UserID { get; set; }

        [DataMember]
        public Guid OverdraftID { get; set; }         

        /// <summary>
        /// Sobre escribe los valores que el usuario pueda haber capturado de forma manual
        /// </summary>
        [DataMember]
        public bool ResetCalculation { get; set; }

        /// <summary>
        /// Si se va a guardar o no el overdraft al final del cálculo
        /// </summary>
        [DataMember]
        public bool SaveOverdraft { get; set; }

        /// <summary>
        /// Para el caso de cálculo Fire and Forget, no tomar en cuenta el borrado de acumulados (posible deadlock)
        /// </summary>
        [DataMember]
        public bool DeleteAccumulates { get; set; }
      
    }
}
