using System.Web.Mvc;
using Xunit;
using xVal.ClientSideValidation;
using xVal.Tests.TestHelpers;
using xVal.Html;

namespace xVal.Tests.HtmlHelpers
{
    public class HtmlHelpersTests
    {
        [Fact]
        public void ValidationMessage_Extends_HtmlHelper()
        {
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            html.ValidationMessage("blah");
        }

        [Fact]
        public void ValidationMessage_Calls_ValidationMessage_On_Active_Validation_Engine()
        {
            // Arrange
            var mockEngine = new Moq.Mock<IClientSideValidationEngine>();
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            mockEngine.Expect(x => x.ValidationMessage(html, "mymodel")).Returns("ok");
            ActiveClientSideValidationEngine.Engine = mockEngine.Object;            

            // Act
            var result = html.ValidationMessage("mymodel");

            // Assert
            Assert.Equal("ok", result);
        }
    }
}