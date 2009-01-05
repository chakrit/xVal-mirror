using System.ComponentModel.DataAnnotations;

namespace xVal.RuleProviders
{
    /// <summary>
    /// To save time, the rule is currently described by a ValidationAttribute property called Rule.
    /// In future, ValidationRule may become an abstract base class for a hierarchy of rule types.
    /// This would break the dependency on System.ComponentModel.DataAnnotations.ValidationAttribute.
    /// </summary>
    public class ValidationRule
    {
        public string PropertyName { get; set; }
        public ValidationAttribute Rule { get; set; }

        public ValidationRule(string propertyName, ValidationAttribute rule)
        {
            PropertyName = propertyName;
            Rule = rule;
        }
    }
}