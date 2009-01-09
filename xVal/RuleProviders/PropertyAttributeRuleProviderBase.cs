using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public abstract class PropertyAttributeRuleProviderBase<TAttribute> : IRuleProvider where TAttribute : Attribute
    {
        public RuleSet GetRulesFromType(Type type)
        {
            // Todo: Consider supporting ICustomAttributeProvider
            var rules = (from prop in type.GetProperties()
                         from att in prop.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>()
                         let validationRule = MakeValidationRuleFromAttribute(att)
                         where validationRule != null
                         select new KeyValuePair<string, RuleBase>(prop.Name, validationRule));
            return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
        }

        protected abstract RuleBase MakeValidationRuleFromAttribute(TAttribute att);
    }
}