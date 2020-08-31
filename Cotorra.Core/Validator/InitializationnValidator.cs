using Cotorra.Schema;
using System;
using System.Collections.Generic;
using System.Text;

namespace Cotorra.Core.Validator
{
    public class InitializationnValidator : IProcessValidator<InitializationParams>
    {
        private readonly List<IValidationObjectRule> createRules = new List<IValidationObjectRule>();

        static InitializationnValidator()
        {
          
        }

        public InitializationnValidator()
        { 
            createRules.Add(new GuidRule("LicenseServiceID", "ID de servicio", 1201)); 
            createRules.Add(new SimpleStringRule("SocialReason", "Razón social ", true, 1, 100, 1204));
            createRules.Add(new SimpleStringRule("RFC", "RFC ", true, 1, 100, 1205));
            createRules.Add(new ObjectNullRule("PayrollCompanyConfiguration", "Configuración de empresa de nómina", 1206));

        }

        public void BeforeProcess(InitializationParams lstObjectsToValidate)
        {
            var validator = new RuleValidatorObject<InitializationParams>();
            validator.ValidateRules(new List<InitializationParams>() { lstObjectsToValidate }, createRules);
        }
    }
}
