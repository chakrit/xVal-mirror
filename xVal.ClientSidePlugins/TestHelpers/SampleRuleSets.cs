using System.Collections.Generic;
using xVal.RuleProviders;
using xVal.Rules;
using System.Linq;

namespace xVal.ClientSidePlugins.TestHelpers
{
    public static class SampleRuleSets
    {
        public static RuleSet Person
        {
            get
            {
                var rules = new List<KeyValuePair<string, RuleBase>> {
                    new KeyValuePair<string, RuleBase>("Name", new RequiredRule { ErrorMessage = "State your name"}),
                    new KeyValuePair<string, RuleBase>("Age", new NumericRangeRule(0, 150) { ErrorMessage = "Age must be within range 0-150"})
                };
                return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
            }
        }
    }
}