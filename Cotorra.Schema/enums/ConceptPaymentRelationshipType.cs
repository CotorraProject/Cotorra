using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum ConceptPaymentRelationshipType
    {
        FiscalAccumulates = 1,
        OtherAccumulates = 2
    }
}
