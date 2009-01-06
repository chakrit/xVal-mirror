using System;
using System.Web.Mvc;
using System.Xml.Linq;

namespace xVal.Html
{
    public static class ValidationHelpers
    {
        public static IValidationConfigFormatter Formatter = new DefaultValidationConfigFormatter();

        public static string ClientSideValidation(this HtmlHelper html, Type modelType)
        {
            return ClientSideValidation(html, modelType, null);
        }

        public static string ClientSideValidation(this HtmlHelper html, Type modelType, string modelName)
        {
            string prefix = modelName == null ? null : modelName + ".";
            return Formatter.FormatRules(ActiveRuleProviders.GetRulesForType(modelType), prefix);
        }

        public static string ClientSideValidation<TModel>(this HtmlHelper html)
        {
            return ClientSideValidation(html, typeof(TModel), null);
        }

        public static string ClientSideValidation<TModel>(this HtmlHelper html, string modelName)
        {
            return ClientSideValidation(html, typeof(TModel), modelName);
        }
    }
}