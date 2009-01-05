using System;
using System.Collections.Generic;

namespace xVal.RuleProviders
{
    public interface IRuleProvider
    {
        IEnumerable<ValidationRule> GetRulesFromType(Type type);
    }
}