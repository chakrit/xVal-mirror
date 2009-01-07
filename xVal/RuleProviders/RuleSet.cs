using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public class RuleSet : IEnumerable<KeyValuePair<string, RuleBase>>
    {
        public static readonly RuleSet Empty = new RuleSet(new object[] { }.ToLookup(x => (string) null, x => (RuleBase) null));

        private readonly ILookup<string, RuleBase> rules;

        public RuleSet(ILookup<string, RuleBase> rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            this.rules = rules;
        }

        public RuleSet(IEnumerable<KeyValuePair<string, RuleBase>> rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            this.rules = rules.ToLookup(x => x.Key, x => x.Value);
        }

        public IEnumerator<KeyValuePair<string, RuleBase>> GetEnumerator()
        {
            return (from grp in rules
                   from rule in grp
                   select new KeyValuePair<string, RuleBase>(grp.Key, rule)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string key)
        {
            return rules.Contains(key);
        }

        public IEnumerable<RuleBase> this[string key]
        {
            get { return rules[key]; }
        }

        public IEnumerable<string> Keys
        {
            get { return rules.Select(x => x.Key); }
        }
    }
}