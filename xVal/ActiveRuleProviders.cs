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
            var rules = (from provider in Providers
                         from rulegroup in provider.GetRulesFromType(type) ?? RuleSet.Empty
                         from rule in rulegroup
                         select new { rulegroup.Key, rule }).ToLookup(x => x.Key, x => x.rule);
            return new RuleSet(rules);
        }
    }
}