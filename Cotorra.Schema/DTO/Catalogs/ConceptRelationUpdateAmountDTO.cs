using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class ConceptRelationUpdateAmountDTO
    {
        public Guid ID { get; set; }
        public decimal AmountApplied { get; set; }
        public bool IsAmountAppliedCapturedByUser { get; set; }
    }

}
