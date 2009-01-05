using System;
using System.Web.Mvc;

namespace xVal.Html
{
    public static class ValidationExtensions
    {
        public static string ValidationMessage(this HtmlHelper html, string modelName)
        {
            if (ActiveClientSideValidationEngine.Engine == null)
                throw new InvalidOperationException("There is no active client-side validation engine. Please assign one to ActiveClientSideValidationEngine.Engine.");

            return ActiveClientSideValidationEngine.Engine.ValidationMessage(html, modelName);
        }
    }
}