using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using xVal.RuleProviders;
using xVal.Rules;

namespace xVal.Html
{
    public class DefaultValidationConfigFormatter : IValidationConfigFormatter
    {
        private const string TagName = "ruleset";
        private const string NewLine = "\r\n";
        private const string Indent = "    ";

        public string FormatRules(RuleSet rules, string rulesetName)
        {
            var tb = new TagBuilder(TagName);
            tb.MergeAttribute("name", rulesetName);

            var rulesBuilder = new StringBuilder();
            var allRules = from key in rules.Keys
                           from rule in rules[key]
                           select new { key, rule };

            if (allRules.Any())
                rulesBuilder.Append(NewLine);
            foreach (var item in allRules) {
                rulesBuilder.Append(Indent);
                rulesBuilder.Append(FormatSingleRule(item.rule, item.key));
                rulesBuilder.Append(NewLine);
            }

            tb.InnerHtml = rulesBuilder.ToString();
            return tb.ToString(allRules.Any() ? TagRenderMode.Normal : TagRenderMode.SelfClosing);
        }

        private static string FormatSingleRule(RuleBase rule, string forField)
        {
            var tb = new TagBuilder(rule.RuleName);
            tb.MergeAttributes(rule.ListParameters());
            tb.MergeAttribute("forfield", forField);
            return tb.ToString(TagRenderMode.SelfClosing);
        }
    }
}