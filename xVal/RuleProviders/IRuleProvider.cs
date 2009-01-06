using System;
using System.Collections.Generic;
using System.Linq;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public interface IRuleProvider
    {
        ILookup<string, RuleBase> GetRulesFromType(Type type);
    }
}