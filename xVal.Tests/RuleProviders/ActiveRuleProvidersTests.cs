using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using xVal.RuleProviders;
using System.ComponentModel.DataAnnotations;

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
            var rules = ActiveRuleProviders.GetRulesForType(arbitraryType);

            // Assert
            Assert.Equal(6, rules.Count);
            Assert.Equal("prop1a", rules[0].PropertyName);
            Assert.Equal("prop1b", rules[1].PropertyName);
            Assert.Equal("prop2", rules[2].PropertyName);
            Assert.Equal("prop3a", rules[3].PropertyName);
            Assert.Equal("prop3b", rules[4].PropertyName);
            Assert.Equal("prop3c", rules[5].PropertyName);
        }

        private IRuleProvider MakeMockRuleProvider(Type forModelType, params string[] rulePropertyNames)
        {
            var mockProvider = new Moq.Mock<IRuleProvider>();
            mockProvider.Expect(x => x.GetRulesFromType(forModelType))
                .Returns(from propName in rulePropertyNames
                         select new ValidationRule(propName, new RequiredAttribute()));
            return mockProvider.Object;
        }
    }
}