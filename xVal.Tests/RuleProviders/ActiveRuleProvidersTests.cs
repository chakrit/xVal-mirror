using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using xVal.RuleProviders;
using System.ComponentModel.DataAnnotations;
using xVal.Rules;

namespace xVal.Tests.RuleProviders
{
    public class ActiveRuleProvidersTests
    {
        [Fact]
        public void Providers_Has_Only_DataAnnotationsRuleProvider_By_Default()
        {
            Assert.Equal(1, ActiveRuleProviders.Providers.Count);
            Assert.IsType<DataAnnotationsRuleProvider>(ActiveRuleProviders.Providers[0]);
        }

        [Fact]
        public void GetRulesForType_Concatenates_Output_From_All_Providers()
        {
            // Arrange
            var arbitraryType = typeof(int);
            var someOtherType = typeof(string);
            var mockProvider1 = MakeMockRuleProvider(arbitraryType, "prop1a", "prop1b");
            var mockProvider2 = MakeMockRuleProvider(arbitraryType, "prop2");
            var mockProvider3 = MakeMockRuleProvider(arbitraryType, "prop3a", "prop3b", "prop3c");
            var mockProvider4 = MakeMockRuleProvider(someOtherType, "this_should_not_be_output");
            ActiveRuleProviders.Providers.Clear();
            ActiveRuleProviders.Providers.Add(mockProvider1);
            ActiveRuleProviders.Providers.Add(mockProvider2);
            ActiveRuleProviders.Providers.Add(mockProvider3);
            ActiveRuleProviders.Providers.Add(mockProvider4);

            // Act
            var rules = ActiveRuleProviders.GetRulesForType(arbitraryType).ToList();

            // Assert
            Assert.Equal(6, rules.Count);
            Assert.Equal("prop1a", rules[0].Key);
            Assert.Equal("prop1b", rules[1].Key);
            Assert.Equal("prop2", rules[2].Key);
            Assert.Equal("prop3a", rules[3].Key);
            Assert.Equal("prop3b", rules[4].Key);
            Assert.Equal("prop3c", rules[5].Key);
        }

        [Fact]
        public void GetRulesForType_Can_Handle_Provider_Returning_NULL()
        {
            // Arrange
            var mockProvider = new Moq.Mock<IRuleProvider>();
            mockProvider.Expect(x => x.GetRulesFromType(typeof(double)))
                        .Returns((ILookup<string, RuleBase>)null);
            ActiveRuleProviders.Providers.Clear();
            ActiveRuleProviders.Providers.Add(mockProvider.Object);

            // Act
            var rules = ActiveRuleProviders.GetRulesForType(typeof (double));

            // Assert
            Assert.NotNull(rules);
            Assert.Empty(rules);
        }

        private static IRuleProvider MakeMockRuleProvider(Type forModelType, params string[] rulePropertyNames)
        {
            var ruleset = rulePropertyNames.ToLookup(x => x, x => (RuleBase)new RequiredRule());
            var mockProvider = new Moq.Mock<IRuleProvider>();
            mockProvider.Expect(x => x.GetRulesFromType(forModelType)).Returns(ruleset);
            return mockProvider.Object;
        }
    }
}