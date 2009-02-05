using System.Linq;
using NHibernate.Validator;
using NHibernate.Validator.Engine;
using Xunit;
using xVal.RuleProviders;
using xVal.Rules;
using xVal.RulesProviders.NHibernateValidator;

namespace xVal.Tests.RuleProviders
{
    public class NHibernateValidatorRulesProviderTests
    {
        [Fact]
        public void ImplementsIRuleProvider()
        {
            new NHibernateValidatorRulesProvider(ValidatorMode.UseXml);
        }

        [Fact]
        public void Detects_Attributes_On_Public_Properties()
        {
            // Arrange
            var provider = new NHibernateValidatorRulesProvider(ValidatorMode.UseAttribute);

            // Act
            var rules = provider.GetRulesFromType(typeof (TestModel));

            // Assert
            Assert.Equal(1, rules.Keys.Count());
            var lengthRule = rules["Name"].First() as StringLengthRule;
            Assert.Equal(3, lengthRule.MinLength);
            Assert.Equal(6, lengthRule.MaxLength);
        }

        private class TestModel
        {
            [Length(3, 6)]
            public string Name { get; set; }
        }
    }
}