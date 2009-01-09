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
                         from rule in GetRulesFromProperty(prop)
                         select new KeyValuePair<string, RuleBase>(prop.Name, rule));
            return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
        }

        protected virtual IEnumerable<RuleBase> GetRulesFromProperty(PropertyInfo propertyInfo)
        {
            return from att in propertyInfo.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>()
                   let validationRule = MakeValidationRuleFromAttribute(att)
                   where validationRule != null
                   select validationRule;
        }

        protected abstract RuleBase MakeValidationRuleFromAttribute(TAttribute att);
    }
}