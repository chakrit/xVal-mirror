using System;
using System.Web.Mvc;
using System.Xml.Linq;

namespace xVal.Html
{
    public static class ValidationHelpers
    {
        public static IValidationConfigFormatter Formatter = new DefaultValidationConfigFormatter();

        public static string ClientSideValidationRules(this HtmlHelper html, Type modelType, string rulesetName)
        {
            if (modelType == null) throw new ArgumentNullException("modelType");
            if (string.IsNullOrEmpty(rulesetName)) throw new ArgumentException("rulesetName");
            return Formatter.FormatRules(ActiveRuleProviders.GetRulesForType(modelType), rulesetName);
        }

        public static string ClientSideValidationRules<TModel>(this HtmlHelper html, string rulesetName)
        {
            return ClientSideValidationRules(html, typeof(TModel), rulesetName);
        }
    }
}