using System;
using System.Text.RegularExpressions;

namespace xVal.Rules
{
    public class RegularExpressionRule : RuleBase
    {
        public string Pattern { get; private set; }
        public RegexOptions Options { get; private set; }
        public RegularExpressionRule(string pattern) : this(pattern, RegexOptions.None) { }

        public RegularExpressionRule(string pattern, RegexOptions options) : base("RegEx")
        {
            if (pattern == null) throw new ArgumentNullException("pattern");
            Pattern = pattern;
            Options = options;
        }

        public override System.Collections.Generic.IDictionary<string, string> ListParameters()
        {
            var result = base.ListParameters();
            result.Add("Pattern", Pattern);

            string options = "";
            if ((Options & RegexOptions.IgnoreCase) == RegexOptions.IgnoreCase) options += "i";
            if ((Options & RegexOptions.Multiline) == RegexOptions.Multiline) options += "m";
            if(options != "")
                result.Add("Options", options);
            
            return result;
        }
    }
}