using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace xVal.RuleProviders
{
    public abstract class PropertyAttributeRuleProviderBase<TAttribute> : IRuleProvider
    {
        public IEnumerable<ValidationRule> GetRulesFromType(Type type)
        {
            // Todo: Consider supporting ICustomAttributeProvider
            return from prop in type.GetProperties()
                   from att in prop.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>()
                   select MakeValidationRuleFromAttribute(prop.Name, att);
        }

        protected abstract ValidationRule MakeValidationRuleFromAttribute(string propertyName, TAttribute att);
    }
}