using CotorraNode.Common.ManageException;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Schema
{
    public class CotorraException: BaseException
    {
        public CotorraException(int errorCode, string code, string message, Exception innerException, List<RuleValidationReg> ruleValidations) : base(errorCode, code, message, innerException)
        {
            this.ValidationInfo = ruleValidations;
        }

        public CotorraException(int errorCode, string code, string message, Exception innerException) : base(errorCode, code, message, innerException)
        {
            
        }

        public CotorraException(int errorCode, string code, string message) : base(errorCode, code, message, null)
        {

        }

        public List<RuleValidationReg> ValidationInfo { get; set; }

    }
}
