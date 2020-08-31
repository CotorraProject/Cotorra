using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class RuleValidationReg
    {
        public Guid ID { get; set; }

        public List<RuleValidation> Validations { get; set; }
    }


    public class RuleValidation
    {
        public Guid ID { get; set; }
        public string Message { get; set; }
        public bool IsValidValue { get; set; }
        public int ErrorCode { get; set; }
        public string Field { get; set; }
        public string ValueSent { get; set; }
    }
}
