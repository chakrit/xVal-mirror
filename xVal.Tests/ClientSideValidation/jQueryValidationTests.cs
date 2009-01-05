using Xunit;
using xVal.ClientSideValidation;
using xVal.Tests.TestHelpers;

namespace xVal.Tests.ClientSideValidation
{
    public class jQueryValidationTests
    {
        [Fact]
        public void Implements_IClientSideValidationEngine()
        {
            IClientSideValidationEngine engine = new jQueryValidationEngine();
        }

        [Fact]
        public void ValidationMessage_Formatted_Correctly_With_No_ModelErrors()
        {
            // Arrange
            var engine = new jQueryValidationEngine();
            var html = new HtmlHelperMocks<object>().HtmlHelper;

            // Act
            var result = engine.ValidationMessage(html, "my.model");

            // Assert
            Assert.Equal("<span class=\"field-validation-error\" forhtml=\"my.model\" generated=\"true\"></span>", result);
        }

        [Fact]
        public void ValidationMessage_Formatted_Correctly_With_Single_ModelError()
        {
            // Arrange
            var engine = new jQueryValidationEngine();
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            html.ViewData.ModelState.AddModelError("my.model", "This is bad");

            // Act
            var result = engine.ValidationMessage(html, "my.model");

            // Assert
            Assert.Equal("<span class=\"field-validation-error\" forhtml=\"my.model\" generated=\"true\">This is bad</span>", result);
        }

        [Fact]
        public void ValidationMessage_Html_Encodes_Message()
        {
            // Arrange
            var engine = new jQueryValidationEngine();
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            html.ViewData.ModelState.AddModelError("my.model", "This is <bad>");

            // Act
            var result = engine.ValidationMessage(html, "my.model");

            // Assert
            Assert.Equal("<span class=\"field-validation-error\" forhtml=\"my.model\" generated=\"true\">This is &lt;bad&gt;</span>", result);
        }
    }
}