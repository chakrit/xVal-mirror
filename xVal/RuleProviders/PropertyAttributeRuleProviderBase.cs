using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public abstract class PropertyAttributeRuleProviderBase<TAttribute> : IRuleProvider
    {
        public RuleSet GetRulesFromType(Type type)
        {
            // Todo: Consider supporting ICustomAttributeProvider
            var rules = (from prop in type.GetProperties()
                         from att in prop.GetCustomAttributes(typeof (TAttribute), true).OfType<TAttribute>()
                         select new { prop.Name, att }).ToLookup(x => x.Name, x => MakeValidationRuleFromAttribute(x.att));
            return new RuleSet(rules);
        }

        protected abstract RuleBase MakeValidationRuleFromAttribute(TAttribute att);
    }
}