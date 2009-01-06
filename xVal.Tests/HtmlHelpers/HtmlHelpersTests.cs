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
            Action<HtmlHelper> test = x => x.ClientSideValidation(Moq.It.IsAny<Type>());
        }

        [Fact]
        public void ClientSideValidation_Helper_Passes_Null_PropertyName_To_Formatter()
        {
            // Arrange
            var arbitraryType = typeof (DateTime);
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<ILookup<string, RuleBase>>(), null))
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidation(arbitraryType, null);

            // Assert
            Assert.Equal("ok", result);
        }

        [Fact]
        public void ClientSideValidation_Helper_Passes_NonNull_PropertyName_Plus_Dot_To_Formatter()
        {
            // Arrange
            var arbitraryType = typeof(DateTime);
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<ILookup<string, RuleBase>>(), "myprop."))
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidation(arbitraryType, "myprop");

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
            var rules = new[] { "someProperty" }.ToLookup(x => x, x => (RuleBase)new RequiredRule());
            ruleProvider.Expect(x => x.GetRulesFromType(arbitraryType)).Returns(rules);
            ActiveRuleProviders.Providers.Clear();
            ActiveRuleProviders.Providers.Add(ruleProvider.Object);

            // Capture params passed to mockFormatter
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            ILookup<string, RuleBase> passedRules = null;
            string passedPrefix = null;
            Action<ILookup<string, RuleBase>, string> callback = (x, y) => {
                passedPrefix = y;
                passedRules = x;
            };
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<ILookup<string, RuleBase>>(), It.IsAny<string>()))
                .Callback(callback)
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidation(arbitraryType, "blah[3].subprop");

            // Assert
            Assert.Equal("ok", result);
            Assert.Equal("blah[3].subprop.", passedPrefix);
            Assert.Equal(1, passedRules.Count);
            Assert.Same(rules["someProperty"].First(), passedRules["someProperty"].First());
        }
    }
}