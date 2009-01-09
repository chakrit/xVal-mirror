using System;
using System.Web.Mvc;
using System.Xml.Linq;
using xVal.RuleProviders;

namespace xVal.Html
{
    public static class ValidationHelpers
    {
        public static IValidationConfigFormatter Formatter = new JsonValidationConfigFormatter();

        public static string ClientSideValidationRules(this HtmlHelper html, Type modelType)
        {
            if (modelType == null) throw new ArgumentNullException("modelType");
            return Formatter.FormatRules(ActiveRuleProviders.GetRulesForType(modelType));
        }

        public static string ClientSideValidationRules<TModel>(this HtmlHelper html)
        {
            return ClientSideValidationRules(html, typeof(TModel));
        }

        public static string ClientSideValidationRules(this HtmlHelper html, RuleSet rules)
        {
            if (rules == null) throw new ArgumentNullException("rules");
            return Formatter.FormatRules(rules);
        }

        public static string ClientSideValidation(this HtmlHelper html, string elementPrefix, RuleSet rules)
        {
            var tb = new TagBuilder("script");
            tb.MergeAttribute("type", "text/javascript");
            elementPrefix = elementPrefix != null
                                ? string.Format("\"{0}\"", elementPrefix)
                                : "null";
            tb.InnerHtml = string.Format("xVal.AttachValidator({0}, {1})", elementPrefix, ClientSideValidationRules(html, rules));
            return tb.ToString(TagRenderMode.Normal);
        }

        public static string ClientSideValidation(this HtmlHelper html, string elementPrefix, Type modelType)
        {
            return ClientSideValidation(html, elementPrefix, ActiveRuleProviders.GetRulesForType(modelType));
        }

        public static string ClientSideValidation(this HtmlHelper html, Type modelType)
        {
            return ClientSideValidation(html, null, modelType);
        }

        public static string ClientSideValidation<TModel>(this HtmlHelper html, string elementPrefix)
        {
            return ClientSideValidation(html, elementPrefix, typeof (TModel));
        }

        public static string ClientSideValidation<TModel>(this HtmlHelper html)
        {
            return ClientSideValidation(html, null, typeof(TModel));
        }
    }
}