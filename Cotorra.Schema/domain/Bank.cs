using CotorraNode.Common.Base.Schema;
using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public class Bank : CatalogEntity
    {
        [DataMember]
        public int Code { get; set; }
    }
}
