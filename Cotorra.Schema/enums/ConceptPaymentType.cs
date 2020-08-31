using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    [DataContract]
    public enum ConceptPaymentType
    {
        TotalAmount = 1,
        Amount1 = 2,
        Amount2 = 3,
        Amount3 = 4,
        Amount4 = 5,
    }
}
