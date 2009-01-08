using System;
using System.Collections;
using System.ComponentModel.DataAnnotations;
using xVal.Rules;

namespace xVal.RuleProviders
{
    // Could be moved out into an external assembly to break the dependency on System.ComponentModel.DataAnnotations
    // (since this is really a plugin)
    public class DataAnnotationsRuleProvider : PropertyAttributeRuleProviderBase<ValidationAttribute>
    {
        protected override RuleBase MakeValidationRuleFromAttribute(ValidationAttribute att)
        {
            RuleBase result = null;

            if (att is RequiredAttribute)
                result = new RequiredRule();
            else if (att is StringLengthAttribute) {
                var sl = (StringLengthAttribute) att;
                result = new StringLengthRule(null, sl.MaximumLength);
            }
            else if (att is RangeAttribute)
            {
                var r = (RangeAttribute) att;
                result = new RangeRule(r.Minimum == null ? (decimal?) null : Convert.ToDecimal(r.Minimum), r.Maximum == null ? (decimal?) null : Convert.ToDecimal(r.Maximum));
            }
            else if (att is DataTypeAttribute)
            {
                var dt = (DataTypeAttribute) att;
                result = new DataTypeRule(ToXValDataType(dt.DataType));
            }

            if(result != null) {
                if(att.ErrorMessage != null)
                    result.ErrorMessage = att.ErrorMessage;
                else {
                    result.ErrorMessageResourceType = att.ErrorMessageResourceType;
                    result.ErrorMessageResourceName = att.ErrorMessageResourceName;
                }
                return result;
            }

            throw new InvalidOperationException("Unknown validation attribute type: " + att.GetType().FullName);
        }

        private static DataTypeRule.DataType ToXValDataType(DataType type)
        {
            switch(type) {
                case DataType.EmailAddress:
                    return DataTypeRule.DataType.EmailAddress;
                default:
                    throw new InvalidOperationException("Unknown data type: " + type.ToString());
            }
        }
    }
}