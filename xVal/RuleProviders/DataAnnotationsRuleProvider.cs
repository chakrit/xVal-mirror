using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace xVal.RuleProviders
{
    // Could be moved out into an external assembly to break the dependency on System.ComponentModel.DataAnnotations
    // (since this is really a plugin)
    public class DataAnnotationsRuleProvider : PropertyAttributeRuleProviderBase<ValidationAttribute>
    {
        protected override ValidationRule MakeValidationRuleFromAttribute(string propertyName, ValidationAttribute att)
        {
            return new ValidationRule(propertyName, att);
        }
    }
}