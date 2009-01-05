using System;
using System.Collections.Generic;
using System.Linq;
using xVal.RuleProviders;

namespace xVal
{
    public static class ActiveRuleProviders
    {
        public static IList<IRuleProvider> Providers = new List<IRuleProvider> {
            new DataAnnotationsRuleProvider()
        };

        public static IList<ValidationRule> GetRulesForType(Type type)
        {
            return (from provider in Providers
                   from rule in provider.GetRulesFromType(type)
                   select rule).ToList();
        }
    }
}