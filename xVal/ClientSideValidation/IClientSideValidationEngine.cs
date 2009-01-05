using System.Web.Mvc;

namespace xVal.ClientSideValidation
{
    public interface IClientSideValidationEngine
    {
        string ValidationMessage(HtmlHelper html, string modelName);
    }
}