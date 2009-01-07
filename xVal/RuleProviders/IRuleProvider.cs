using System;

namespace xVal.RuleProviders
{
    public interface IRuleProvider
    {
        RuleSet GetRulesFromType(Type type);
    }
}