using System;
using System.Collections.Generic;
using System.Linq;
using xVal.RuleProviders;
using xVal.Rules;

namespace xVal
{
    public static class ActiveRuleProviders
    {
        private static readonly ILookup<string, RuleBase> EmptyRuleSet = new string[] { }.ToLookup(x => (string)null, xVal => (RuleBase)null);

        public static IList<IRuleProvider> Providers = new List<IRuleProvider> {
            new DataAnnotationsRuleProvider()
        };

        public static ILookup<string, RuleBase> GetRulesForType(Type type)
        {
            return (from provider in Providers
                    from rulegroup in provider.GetRulesFromType(type) ?? EmptyRuleSet
                    from rule in rulegroup
                    select new { rulegroup.Key, rule }).ToLookup(x => x.Key, x => x.rule);
        }
    }
}