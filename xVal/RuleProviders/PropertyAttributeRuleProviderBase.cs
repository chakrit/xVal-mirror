using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public abstract class PropertyAttributeRuleProviderBase<TAttribute> : IRulesProvider where TAttribute : Attribute
    {
        public RuleSet GetRulesFromType(Type type)
        {
            var typeDescriptor = GetTypeDescriptionProvider(type).GetTypeDescriptor(type);
            var rules = (from prop in typeDescriptor.GetProperties().Cast<PropertyDescriptor>()
                         from rule in GetRulesFromProperty(prop)
                         select new KeyValuePair<string, RuleBase>(prop.Name, rule));
            return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
        }

        protected virtual IEnumerable<RuleBase> GetRulesFromProperty(PropertyDescriptor propertyDescriptor)
        {
            return from att in propertyDescriptor.Attributes.OfType<TAttribute>()
                   let validationRule = MakeValidationRuleFromAttribute(att)
                   where validationRule != null
                   select validationRule;
        }

        protected abstract RuleBase MakeValidationRuleFromAttribute(TAttribute att);

        protected virtual TypeDescriptionProvider GetTypeDescriptionProvider(Type type)
        {
            return TypeDescriptor.GetProvider(type);
        }
    }
}