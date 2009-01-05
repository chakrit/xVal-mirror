using Xunit;
using xVal.ClientSideValidation;

namespace xVal.Tests.ClientSideValidation
{
    public class IClientSideValidationEngineTests
    {
        [Fact]
        public void Interface_Has_ValidationMessage_Method()
        {
            var mockEngine = new Moq.Mock<IClientSideValidationEngine>();
            mockEngine.Expect(x => x.ValidationMessage(null, null));
        }
    }
}