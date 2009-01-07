using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public class RuleSet : ILookup<string, RuleBase>
    {
        public static readonly RuleSet Empty = new RuleSet(new object[] { }.ToLookup(x => (string) null, x => (RuleBase) null));

        private readonly ILookup<string, RuleBase> rules;

        public RuleSet(ILookup<string, RuleBase> rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            this.rules = rules;
        }

        public IEnumerator<IGrouping<string, RuleBase>> GetEnumerator()
        {
            return rules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Contains(string key)
        {
            return rules.Contains(key);
        }

        public int Count
        {
            get { return rules.Count; }
        }

        public IEnumerable<RuleBase> this[string key]
        {
            get { return rules[key]; }
        }
    }
}