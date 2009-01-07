using System;
using System.Linq;
using System.Web.Mvc;
using Moq;
using Xunit;
using xVal.RuleProviders;
using xVal.Rules;
using xVal.Tests.TestHelpers;
using xVal.Html;

namespace xVal.Tests.HtmlHelpers
{
    public class HtmlHelpersTests
    {
        [Fact]
        public void ClientSideValidation_Extends_HtmlHelper()
        {
            Action<HtmlHelper> test = x => x.ClientSideValidationRules(Moq.It.IsAny<Type>(), "myrules");
        }

        [Fact]
        public void ClientSideValidation_Helper_Passes_Ruleset_Name_To_Formatter()
        {
            // Arrange
            var arbitraryType = typeof(DateTime);
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<RuleSet>(), "myprop"))
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidationRules(arbitraryType, "myprop");

            // Assert
            Assert.Equal("ok", result);
        }

        [Fact]
        public void ClientSideValidation_Helper_Passes_ActiveRuleProvider_Output_To_Formatter()
        {
            // Arrange
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var arbitraryType = typeof (DateTime);
            var ruleProvider = new Moq.Mock<IRuleProvider>();
            var rules = new RuleSet(new[] { "someProperty" }.ToLookup(x => x, x => (RuleBase)new RequiredRule()));
            ruleProvider.Expect(x => x.GetRulesFromType(arbitraryType)).Returns(rules);
            ActiveRuleProviders.Providers.Clear();
            ActiveRuleProviders.Providers.Add(ruleProvider.Object);

            // Capture params passed to mockFormatter
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            RuleSet passedRules = null;
            string passedPrefix = null;
            Action<RuleSet, string> callback = (x, y) =>
            {
                passedPrefix = y;
                passedRules = x;
            };
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<RuleSet>(), It.IsAny<string>()))
                .Callback(callback)
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidationRules(arbitraryType, "my.lovely.rules");

            // Assert
            Assert.Equal("ok", result);
            Assert.Equal("my.lovely.rules", passedPrefix);
            Assert.Equal(1, passedRules.Keys.Count());
            Assert.Same(rules["someProperty"].Single(), passedRules["someProperty"].First());
        }
    }
}