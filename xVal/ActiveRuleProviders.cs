using System;
using System.Collections.Generic;
using System.Linq;
using xVal.RuleProviders;
using xVal.Rules;

namespace xVal
{
    public static class ActiveRuleProviders
    {
        public static IList<IRuleProvider> Providers = new List<IRuleProvider> {
            new DataAnnotationsRuleProvider()
        };

        public static RuleSet GetRulesForType(Type type)
        {
            var rules = Providers.SelectMany(x => x.GetRulesFromType(type) ?? RuleSet.Empty);
            return new RuleSet(rules);
        }
    }
}