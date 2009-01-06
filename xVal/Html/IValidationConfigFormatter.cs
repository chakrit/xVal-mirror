using System.Linq;
using xVal.Rules;

namespace xVal.Html
{
    public interface IValidationConfigFormatter
    {
        string FormatRules(ILookup<string, RuleBase> rules, string prefix);
    }
}