using System;
using System.Web.Mvc;
using System.Xml.Linq;

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
    }
}