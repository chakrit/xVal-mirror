using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text.RegularExpressions;
using System.Threading;
using Xunit;
using xVal.RuleProviders;
using xVal.Rules;
using xVal.Tests.TestHelpers;

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
            var rules = provider.GetRulesFromType(typeof (TestModel));

            // Assert the right set of rules were found
            Assert.Equal(3, rules.Keys.Count());
            Assert.Equal(2, rules["PublicProperty"].Count());
            Assert.NotEmpty(rules["PublicProperty"].OfType<RequiredRule>());
            Assert.NotEmpty(rules["PublicProperty"].OfType<RangeRule>());

            // Check attributes properties were retained
            var stringLengthRule = (StringLengthRule)rules["ReadonlyProperty"].First();
            Assert.Equal(3, stringLengthRule.MaxLength);
            Assert.Equal("Too long", stringLengthRule.ErrorMessage);
            var emailAddressRule = (DataTypeRule) rules["WriteonlyProperty"].First();
            Assert.Equal(DataTypeRule.DataType.EmailAddress, emailAddressRule.Type);
            Assert.Equal(typeof(TestResources), emailAddressRule.ErrorMessageResourceType);
            Assert.Equal("TestResourceItem", emailAddressRule.ErrorMessageResourceName);
        }

        [Fact]
        public void Converts_RequiredAttribute_To_RequiredRule()
        {
            TestConversion<RequiredAttribute, RequiredRule>();
        }

        [Fact] 
        public void Converts_StringLengthAttribute_To_StringLengthRule()
        {
            var rule = TestConversion<StringLengthAttribute, StringLengthRule>(5);
            Assert.Equal(5, rule.MaxLength);
            Assert.Null(rule.MinLength);
        }

        [Fact]
        public void Converts_RangeAttribute_Int_To_RangeRule()
        {
            var rule = TestConversion<RangeAttribute, RangeRule>((int)3, (int)6);
            Assert.Equal(3, Convert.ToInt32(rule.Min));
            Assert.Equal(6, Convert.ToInt32(rule.Max));
        }

        [Fact]
        public void Converts_RangeAttribute_Double_To_RangeRule()
        {
            var rule = TestConversion<RangeAttribute, RangeRule>((double)3.492, (double)6.32);
            Assert.Equal(3.492, Convert.ToDouble(rule.Min));
            Assert.Equal(6.32, Convert.ToDouble(rule.Max));
        }

        [Fact]
        public void Converts_RangeAttribute_String_To_RangeRule()
        {
            var rule = TestConversion<RangeAttribute, RangeRule>(typeof(string), "aaa", "zzz");
            Assert.Equal("aaa", Convert.ToString(rule.Min));
            Assert.Equal("zzz", Convert.ToString(rule.Max));
        }

        [Fact]
        public void Converts_RangeAttribute_DateTime_To_RangeRule()
        {
            var min = new DateTime(2003, 01, 23, 05, 03, 10);
            var max = new DateTime(2008, 06, 3);
            var rule = TestConversion<RangeAttribute, RangeRule>(typeof(DateTime), min.ToString(), max.ToString());
            Assert.Equal(min, Convert.ToDateTime(rule.Min));
            Assert.Equal(max, Convert.ToDateTime(rule.Max));    
        }

        [Fact]
        public void Converts_DataTypeAttribute_Email_To_DataTypeRule()
        {
            var rule = TestConversion<DataTypeAttribute, DataTypeRule>(DataType.EmailAddress);
            Assert.Equal(DataTypeRule.DataType.EmailAddress, rule.Type);
        }

        [Fact]
        public void Converts_DataTypeAttribute_DateTime_To_DataTypeRule()
        {
            var rule = TestConversion<DataTypeAttribute, DataTypeRule>(DataType.DateTime);
            Assert.Equal(DataTypeRule.DataType.DateTime, rule.Type);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Date_To_DataTypeRule()
        {
            var rule = TestConversion<DataTypeAttribute, DataTypeRule>(DataType.Date);
            Assert.Equal(DataTypeRule.DataType.Date, rule.Type);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Currency_To_DataTypeRule()
        {
            var rule = TestConversion<DataTypeAttribute, DataTypeRule>(DataType.Currency);
            Assert.Equal(DataTypeRule.DataType.Currency, rule.Type);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Time_To_RegularExpressionRule()
        {
            var rule = TestConversion<DataTypeAttribute, RegularExpressionRule>(DataType.Time);
            Assert.Equal(RegularExpressionRule.Regex_Time, rule.Pattern);
            Assert.Equal(RegexOptions.IgnoreCase, rule.Options);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Duration_To_RegularExpressionRule()
        {
            var rule = TestConversion<DataTypeAttribute, RegularExpressionRule>(DataType.Duration);
            Assert.Equal(RegularExpressionRule.Regex_Duration, rule.Pattern);
            Assert.Equal(RegexOptions.IgnoreCase, rule.Options);
        }

        [Fact]
        public void Converts_DataTypeAttribute_PhoneNumber_To_RegularExpressionRule()
        {
            var rule = TestConversion<DataTypeAttribute, RegularExpressionRule>(DataType.PhoneNumber);
            Assert.Equal(RegularExpressionRule.Regex_USPhoneNumber, rule.Pattern);
            Assert.Equal(RegexOptions.IgnoreCase, rule.Options);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Url_To_RegularExpressionRule()
        {
            var rule = TestConversion<DataTypeAttribute, RegularExpressionRule>(DataType.Url);
            Assert.Equal(RegularExpressionRule.Regex_Url, rule.Pattern);
            Assert.Equal(RegexOptions.IgnoreCase, rule.Options);
        }

        [Fact]
        public void Ignores_Other_DataType()
        {
            // Arrange
            var provider = new DataAnnotationsRuleProvider();
            Type testType = RulesProviderTestHelpers.EmitTestType(typeof(DataTypeAttribute), new object[] { DataType.Custom });

            // Act
            var rules = provider.GetRulesFromType(testType);

            // Assert
            Assert.Empty(rules.Keys);
        }

        [Fact]
        public void Converts_RegularExpressionAttribute_To_RegularExpressionRule()
        {
            var rule = TestConversion<RegularExpressionAttribute, RegularExpressionRule>("somepattern");
            Assert.Equal("somepattern", rule.Pattern);
            Assert.Equal(RegexOptions.None, rule.Options);
        }

        private static TRule TestConversion<TAttribute, TRule>(params object[] attributeConstructorParams)
            where TAttribute : ValidationAttribute
            where TRule : RuleBase
        {
            return RulesProviderTestHelpers.TestConversion<TAttribute, TRule>(new DataAnnotationsRuleProvider(), attributeConstructorParams);
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