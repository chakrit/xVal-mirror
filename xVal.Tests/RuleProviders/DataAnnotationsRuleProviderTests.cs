using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;
using xVal.RuleProviders;

namespace xVal.Tests.RuleProviders
{
    public class DataAnnotationsRuleProviderTests
    {
        [Fact]
        public void ImplementsIRuleProvider()
        {
            IRuleProvider instance = new DataAnnotationsRuleProvider();
        }

        [Fact]
        public void FindsValidationAttributesAttachedToPublicProperties()
        {
            // Arrange
            var provider = new DataAnnotationsRuleProvider();

            // Act
            var rules = provider.GetRulesFromType(typeof (TestModel)).ToList();
            var rulesByPropertyName = rules.ToLookup(
                x => x.PropertyName,
                x => x.Rule
                );

            // Assert the right set of rules were found
            Assert.Equal(4, rules.Count);
            Assert.Equal(2, rulesByPropertyName["PublicProperty"].Count());
            Assert.NotEmpty(rulesByPropertyName["PublicProperty"].OfType<RequiredAttribute>());
            Assert.NotEmpty(rulesByPropertyName["PublicProperty"].OfType<RangeAttribute>());

            // Check attributes properties were retained
            var stringLengthRule = (StringLengthAttribute)rulesByPropertyName["ReadonlyProperty"].First();
            Assert.Equal(3, stringLengthRule.MaximumLength);
            Assert.Equal("Too long", stringLengthRule.ErrorMessage);
            var emailAddressRule = (DataTypeAttribute) rulesByPropertyName["WriteonlyProperty"].First();
            Assert.Equal(DataType.EmailAddress, emailAddressRule.DataType);
            Assert.Equal(typeof(TestResources), emailAddressRule.ErrorMessageResourceType);
            Assert.Equal("TestResourceItem", emailAddressRule.ErrorMessageResourceName);
        }

        private class TestModel
        {
            [Required]
            [Range(5, 10)]
            public object PublicProperty { get; set; }

            [StringLength(3, ErrorMessage = "Too long")]
            public object ReadonlyProperty { get; private set; }

            [DataType(DataType.EmailAddress, ErrorMessageResourceType = typeof (TestResources), ErrorMessageResourceName = "TestResourceItem")]
            public object WriteonlyProperty { private get; set; }

            public object PropertyWithNoValidationAttributes { get; set; }

            [Required] // Shouldn't be detected
                private object PrivateProperty { get; set; }
        }

        private class TestResources
        {
            public static string TestResourceItem { get; set; }
        }
    }
}