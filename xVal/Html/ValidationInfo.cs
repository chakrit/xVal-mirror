using System;
using System.Collections.Generic;
using System.Web.Mvc;
using xVal.RuleProviders;
using xVal.Rules;
using System.Linq;

namespace xVal.Html
{
    /// <summary>
    /// Represents a validation configuration that can be rendered to the client
    /// </summary>
    public class ValidationInfo
    {
        public static IValidationConfigFormatter Formatter = new JsonValidationConfigFormatter();

        private RuleSet rules;
        private readonly List<KeyValuePair<string, Rule>> addedRules = new List<KeyValuePair<string, Rule>>();
        private readonly bool attachValidator;
        private readonly string elementPrefix;

        public ValidationInfo(RuleSet rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            this.rules = rules;
        }

        public ValidationInfo(RuleSet rules, string attachToElementPrefix) : this(rules)
        {
            attachValidator = true;
            elementPrefix = attachToElementPrefix;
        }

        public ValidationInfo AddRule(string propertyName, Rule rule)
        {
            addedRules.Add(new KeyValuePair<string, Rule>(propertyName, rule));
            return this;
        }

        public override string ToString()
        {
            MergeAddedRulesIntoRuleSet();
            var formattedRules = Formatter.FormatRules(rules);
            if (!attachValidator)
                return formattedRules;
            else {
                var tb = new TagBuilder("script");
                tb.MergeAttribute("type", "text/javascript");
                var elementPrefixOrNull = elementPrefix == null ? "null" : string.Format("\"{0}\"", elementPrefix);
                tb.InnerHtml = string.Format("xVal.AttachValidator({0}, {1})", elementPrefixOrNull, formattedRules);
                return tb.ToString(TagRenderMode.Normal);
            }
        }

        private void MergeAddedRulesIntoRuleSet()
        {
            if(addedRules.Count > 0) {
                var addedRuleSet = new RuleSet(addedRules.ToLookup(x => x.Key, x => x.Value));
                rules = new RuleSet(new[] { rules, addedRuleSet });
                addedRules.Clear();
            }
        }
    }
}