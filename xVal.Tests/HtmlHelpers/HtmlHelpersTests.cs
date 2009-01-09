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
        public void ClientSideValidationRules_Extends_HtmlHelper()
        {
            Action<HtmlHelper> test = x => x.ClientSideValidationRules(Moq.It.IsAny<Type>());
        }

        [Fact]
        public void ClientSideValidationRules_Helper_Passes_Ruleset_Name_To_Formatter()
        {
            // Arrange
            var arbitraryType = typeof(DateTime);
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<RuleSet>()))
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidationRules(arbitraryType);

            // Assert
            Assert.Equal("ok", result);
        }

        [Fact]
        public void ClientSideValidationRules_Helper_Passes_ActiveRuleProvider_Output_To_Formatter()
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
            Action<RuleSet> callback = x =>
            {
                passedRules = x;
            };
            mockFormatter.Expect(x => x.FormatRules(It.IsAny<RuleSet>()))
                .Callback(callback)
                .Returns("ok");
            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidationRules(arbitraryType);

            // Assert
            Assert.Equal("ok", result);
            Assert.Equal(1, passedRules.Keys.Count());
            Assert.Same(rules["someProperty"].Single(), passedRules["someProperty"].First());
        }

        [Fact]
        public void ClientSideValidationRules_Can_Take_Explicit_RuleSet_And_Passes_It_To_Formatter()
        {
            // Arrange
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var rules = new RuleSet(new[] { "someProperty" }.ToLookup(x => x, x => (RuleBase)new RequiredRule()));

            // Capture params passed to mockFormatter
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(rules)).Returns("ok");

            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidationRules(rules);

            // Assert
            Assert.Equal("ok", result);
        }

        [Fact]
        public void ClientSideValidation_Extends_HtmlHelper()
        {
            Action<HtmlHelper> test = x => x.ClientSideValidation(typeof(string));
        }

        [Fact]
        public void ClientSideValidation_Wraps_Output_From_Rule_Formatter()
        {
            // Arrange
            var html = new HtmlHelperMocks<object>().HtmlHelper;
            var rules = new RuleSet(new[] { "someProperty" }.ToLookup(x => x, x => (RuleBase)new RequiredRule()));

            // Capture params passed to mockFormatter
            var mockFormatter = new Moq.Mock<IValidationConfigFormatter>(MockBehavior.Strict);
            mockFormatter.Expect(x => x.FormatRules(rules)).Returns("{rulesWouldGoHere}");

            ValidationHelpers.Formatter = mockFormatter.Object;

            // Act
            var result = html.ClientSideValidation("my.prefix", rules);

            // Assert
            Assert.Equal(@"<script type=""text/javascript"">xVal.AttachValidator(""my.prefix"", {rulesWouldGoHere})</script>", result);
        }
    }
}