using Xunit;
using xVal.ClientSideValidation;

namespace xVal.Tests.ClientSideValidation
{
    public class ActiveClientSideValidationEngineTests
    {
        [Fact]
        public void DefaultEngineIs_jQueryValidationEngine()
        {
            Assert.IsType<jQueryValidationEngine>(ActiveClientSideValidationEngine.Engine);
        }
    }
}