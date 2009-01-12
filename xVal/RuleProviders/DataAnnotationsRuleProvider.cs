using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;
using xVal.Rules;
using System.Linq;

namespace xVal.RuleProviders
{
    // Could be moved out into an external assembly to break the dependency on System.ComponentModel.DataAnnotations
    // (since this is really a plugin)
    public class DataAnnotationsRuleProvider : PropertyAttributeRuleProviderBase<ValidationAttribute>
    {
        private readonly Func<Type, TypeDescriptionProvider> metadataProviderFactory; // Yes, it's a factory factory factory. Just trying to be consistent with the Dynamic Data API (http://mattberseth.com/blog/2008/08/dynamic_data_and_custom_metada.html)
        private static readonly Type[] NumericTypes = new[] { typeof(int), typeof(double), typeof(decimal), typeof(float) };

        public DataAnnotationsRuleProvider()
            : this(x => new AssociatedMetadataTypeTypeDescriptionProvider(x))
        {
        }

        public DataAnnotationsRuleProvider(Func<Type, TypeDescriptionProvider> metadataProviderFactory)
        {
            this.metadataProviderFactory = metadataProviderFactory;
        }

        protected override TypeDescriptionProvider GetTypeDescriptionProvider(Type type)
        {
            return metadataProviderFactory(type);
        }

        protected override IEnumerable<RuleBase> GetRulesFromProperty(PropertyDescriptor propertyDescriptor)
        {
            return base.GetRulesFromProperty(propertyDescriptor)
                   .Union(GetNumericValueTypeRulesFromProperty(propertyDescriptor));
        }

        private static IEnumerable<RuleBase> GetNumericValueTypeRulesFromProperty(PropertyDescriptor propertyDescriptor)
        {
            // System.ComponentModel.DataAnnotations doesn't have any attribute to represent "int" or "double",
            // so we'll infer it directly from the property type
            if (Array.IndexOf(NumericTypes, UnwrapIfNullable(propertyDescriptor.PropertyType)) >= 0) {
                if (propertyDescriptor.PropertyType == typeof(int))
                    yield return new DataTypeRule(DataTypeRule.DataType.Integer);
                else
                    yield return new DataTypeRule(DataTypeRule.DataType.Decimal);
            }
        }

        private static Type UnwrapIfNullable(Type type)
        {
            return Nullable.GetUnderlyingType(type) ?? type;
        }

        protected override RuleBase MakeValidationRuleFromAttribute(ValidationAttribute att)
        {
            RuleBase result = null;

            if (att is RequiredAttribute)
                result = new RequiredRule();
            else if (att is StringLengthAttribute)
                result = new StringLengthRule(null, ((StringLengthAttribute) att).MaximumLength);
            else if (att is RangeAttribute)
                result = ConvertRangeAttribute((RangeAttribute) att);
            else if (att is DataTypeAttribute)
                result = ConvertDataTypeAttribute((DataTypeAttribute) att);
            else if (att is RegularExpressionAttribute)
                result = new RegularExpressionRule(((RegularExpressionAttribute) att).Pattern);

            if(result != null) {
                ApplyErrorMessage(att, result);
                return result;
            }

            return null;
        }

        private static void ApplyErrorMessage(ValidationAttribute att, RuleBase result)
        {
            if(att.ErrorMessage != null)
                result.ErrorMessage = att.ErrorMessage;
            else {
                result.ErrorMessageResourceType = att.ErrorMessageResourceType;
                result.ErrorMessageResourceName = att.ErrorMessageResourceName;
            }
        }

        private RuleBase ConvertDataTypeAttribute(DataTypeAttribute dt)
        {
            // Is this one that should be handled as a RegEx?
            string regEx = ToRegEx(dt.DataType);
            if (regEx != null)
                return new RegularExpressionRule(regEx, RegexOptions.IgnoreCase);
            // No, it must be one we have a native type for
            var xValDataType = ToXValDataType(dt.DataType);
            if (xValDataType != DataTypeRule.DataType.Text) // Ignore "text" - nothing to validate
                return new DataTypeRule(xValDataType);
            return null;
        }

        private static RuleBase ConvertRangeAttribute(RangeAttribute r)
        {
            if (r.OperandType == typeof (string))
                return new RangeRule(Convert.ToString(r.Minimum), Convert.ToString(r.Maximum));
            else if (r.OperandType == typeof (DateTime))
                return new RangeRule(r.Minimum == null ? (DateTime?) null : Convert.ToDateTime(r.Minimum), r.Maximum == null ? (DateTime?) null : Convert.ToDateTime(r.Maximum));
            else if (Array.IndexOf(NumericTypes, r.OperandType) >= 0)
                return new RangeRule(r.Minimum == null ? (decimal?) null : Convert.ToDecimal(r.Minimum), r.Maximum == null ? (decimal?) null : Convert.ToDecimal(r.Maximum));
            else // Can't compare any other type
                return null;
        }

        private string ToRegEx(DataType dataType)
        {
            switch (dataType) {
                case DataType.Time:
                    return RegularExpressionRule.Regex_Time;
                case DataType.Duration:
                    return RegularExpressionRule.Regex_Duration;
                case DataType.PhoneNumber:
                    return RegularExpressionRule.Regex_USPhoneNumber;
                case DataType.Url:
                    return RegularExpressionRule.Regex_Url;
                default:
                    return null;
            }
        }

        private static DataTypeRule.DataType ToXValDataType(DataType type)
        {
            switch(type) {
                case DataType.DateTime:
                    return DataTypeRule.DataType.DateTime;
                case DataType.Date:
                    return DataTypeRule.DataType.Date;
                case DataType.Currency:
                    return DataTypeRule.DataType.Currency;
                case DataType.EmailAddress:
                    return DataTypeRule.DataType.EmailAddress;
                case DataType.Custom:
                case DataType.Text:
                case DataType.Html:
                case DataType.MultilineText:
                case DataType.Password:
                    return DataTypeRule.DataType.Text;
                default:
                    throw new InvalidOperationException("Unknown data type: " + type.ToString());
            }
        }
    }
}