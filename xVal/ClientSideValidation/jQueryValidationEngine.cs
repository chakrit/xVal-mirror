using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace xVal.ClientSideValidation
{
    public class jQueryValidationEngine : IClientSideValidationEngine
    {
        private const string CssClass_ValidationMessage = "field-validation-error";
        private const string ValidationMessageFormat = "<span class=\"{0}\" forhtml=\"{1}\" generated=\"true\">{2}</span>";

        public string ValidationMessage(HtmlHelper html, string modelName)
        {
            string errorMarkup = null;
            if(html.ViewData.ModelState.ContainsKey(modelName)) {
                var modelStateEntry = html.ViewData.ModelState[modelName];
                if(modelStateEntry.Errors.Count > 0) {
                    errorMarkup = html.Encode(modelStateEntry.Errors[0].ErrorMessage);
                }
            }

            return string.Format(ValidationMessageFormat, CssClass_ValidationMessage, modelName, errorMarkup ?? "");
        }
    }
}