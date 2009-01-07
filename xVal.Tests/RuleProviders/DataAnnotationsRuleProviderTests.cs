using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;
using Xunit;
using xVal.RuleProviders;
using xVal.Rules;

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
            Assert.NotEmpty(rules["PublicProperty"].OfType<NumericRangeRule>());

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
        public void Converts_RangeAttribute_To_NumericRangeRule()
        {
            var rule = TestConversion<RangeAttribute, NumericRangeRule>(3, 6);
            Assert.Equal(3, rule.Min);
            Assert.Equal(6, rule.Max);
        }

        [Fact]
        public void Converts_DataTypeAttribute_Email_To_DataTypeRule()
        {
            var rule = TestConversion<DataTypeAttribute, DataTypeRule>(DataType.EmailAddress);
            Assert.Equal(DataTypeRule.DataType.EmailAddress, rule.Type);
        }

        private TRule TestConversion<TAttribute, TRule>(params object[] attributeConstructorParams) where TAttribute: ValidationAttribute where TRule : RuleBase
        {
            // Arrange
            var provider = new DataAnnotationsRuleProvider();
            Type testType = EmitTestType(typeof(TAttribute), attributeConstructorParams);

            // Act
            var rules = provider.GetRulesFromType(testType);

            // Assert
            Assert.Equal(1, rules.Keys.Count());
            var ruleBase = rules["testProperty"].Single();
            Assert.IsType<TRule>(ruleBase);
            return (TRule)ruleBase;
        }

        private Type EmitTestType(Type attributeType, object[] attributeConstructorParams)
        {
            var assembly = Thread.GetDomain().DefineDynamicAssembly(new AssemblyName("testAssembly"), AssemblyBuilderAccess.Run);
            var moduleBuilder = assembly.DefineDynamicModule("testModule");
            var typeBuilder = moduleBuilder.DefineType("testType");
            var attributeConstructorParamTypes = attributeConstructorParams.Select(x => x.GetType()).ToArray();
            var customAttributeBuilder = new CustomAttributeBuilder(attributeType.GetConstructor(attributeConstructorParamTypes), attributeConstructorParams);
            var prop = typeBuilder.DefineProperty("testProperty", PropertyAttributes.None, typeof (string), null);
            prop.SetCustomAttribute(customAttributeBuilder);
            var getMethod = typeBuilder.DefineMethod("get_testProperty", MethodAttributes.Public, typeof(string), null);
            var getMethodILBuilder = getMethod.GetILGenerator();
            getMethodILBuilder.Emit(OpCodes.Ret);
            prop.SetGetMethod(getMethod);
            return typeBuilder.CreateType();
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