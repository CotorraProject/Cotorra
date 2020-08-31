using CotorraNode.Common.Base.Schema;
using System; 
using System.Runtime.Serialization; 

namespace Cotorra.Schema
{

    [DataContract]
    [Serializable]
    public class VacationDaysOff : DataEntityBase
    {

        [DataMember]
        public DateTime Date { get; set; }

        [DataMember]
        public Guid InstanceID { get; set; }


        [DataMember]
        public Guid VacationID { get; set; }


        [DataMember]
        public virtual Vacation Vacation { get; set; }

    }
}