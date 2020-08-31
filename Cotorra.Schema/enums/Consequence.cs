using System.Runtime.Serialization;

namespace Cotorra.Schema
{
    /// <summary>
    /// Consequence
    /// </summary>
    [DataContract]
    public enum Consequence
    {
        None = 0,
        TemporalyInhability = 1,
        ProvisionalInitialEvaluation = 2,
        DefinitiveInitialEvaluation = 3,
        Death = 4,
        Relapse = 5,
        PostDischargeValuation = 6,
        ProvisionalRevaluation = 7,
        RelapseWithoutMedicalDischarge = 8,
        DefinitiveRevaluation = 9
    }
}
