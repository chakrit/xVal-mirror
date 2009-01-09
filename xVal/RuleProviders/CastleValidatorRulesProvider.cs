using System;
using System.Collections.Generic;
using System.Reflection;
using Castle.Components.Validator;
using System.Linq;
using xVal.Rules;

namespace xVal.RuleProviders
{
    public class CastleValidatorRulesProvider : IRuleProvider
    {
        private readonly IValidatorRegistry registry;
        private readonly ValidatorRunner runner;

        public CastleValidatorRulesProvider()
        {
            registry = new CachedValidationRegistry();
            runner = new ValidatorRunner(registry);
        }

        public RuleSet GetRulesFromType(Type type)
        {
            var validators = registry.GetValidators(runner, type, RunWhen.Everytime);
            var allRules = from val in validators
                           from rule in ConvertToXValRules(val)
                           select new KeyValuePair<string, RuleBase>(val.Property.Name, rule);
            return new RuleSet(allRules.ToLookup(x => x.Key, x => x.Value));
        }

        private static IEnumerable<RuleBase> ConvertToXValRules(IValidator validator)
        {
            var result = new List<RuleBase>();

            if (validator is NonEmptyValidator)
                result.Add(new RequiredRule());
            else if (validator is CreditCardValidator)
                result.Add(new DataTypeRule(DataTypeRule.DataType.CreditCardLuhn));
            else if(validator is DateTimeValidator) {
                result.Add(new RequiredRule());
                result.Add(new DataTypeRule(DataTypeRule.DataType.DateTime));
            }
            else if (validator is DateValidator) {
                result.Add(new RequiredRule());
                result.Add(new DataTypeRule(DataTypeRule.DataType.Date));
            }
            else if (validator is IntegerValidator) {
                result.Add(new RequiredRule());
                result.Add(new DataTypeRule(DataTypeRule.DataType.Integer));
            }
            else if ((validator is DecimalValidator) || (validator is DoubleValidator) || (validator is SingleValidator)) {
                result.Add(new RequiredRule());
                result.Add(new DataTypeRule(DataTypeRule.DataType.Decimal));
            } else if(validator is LengthValidator) {
                var lengthRule = ConstructStringLengthRule((LengthValidator)validator);
                if (lengthRule != null)
                    result.Add(lengthRule);
            }
            else if (validator is RangeValidator) {
                var rangeRule = ConstructRangeRule((RangeValidator) validator);
                if (rangeRule != null)
                    result.Add(rangeRule);
            } 
            else if (validator is RegularExpressionValidator) {
                var regularExpressionValidator = (RegularExpressionValidator)validator;
                result.Add(new RegularExpressionRule(regularExpressionValidator.Expression, regularExpressionValidator.RegexRule.Options));
            }

            return result; 
        }

        private static StringLengthRule ConstructStringLengthRule(LengthValidator lengthValidator)
        {
            if(lengthValidator.ExactLength != int.MinValue)
                return new StringLengthRule(lengthValidator.ExactLength, lengthValidator.ExactLength);
            else if((lengthValidator.MinLength != int.MaxValue) || (lengthValidator.MaxLength != int.MaxValue)) {
                return new StringLengthRule(
                               /* Min length */ lengthValidator.MinLength == int.MinValue ? (int?)null : lengthValidator.MinLength,
                                                /* Min length */ lengthValidator.MaxLength == int.MaxValue ? (int?)null : lengthValidator.MaxLength
                               );
            }
            return null;
        }

        // Due to an annoying inconsistency in Castle Validator, there's no way to determine the 
        // min and max values for a RangeValidator other than by peeking at a private field.
        // (All other validators expose a useful public description of themselves.)
        private readonly static FieldInfo rangeValidatorMinField = typeof(RangeValidator).GetField("min", BindingFlags.Instance | BindingFlags.NonPublic);
        private readonly static FieldInfo rangeValidatorMaxField = typeof(RangeValidator).GetField("max", BindingFlags.Instance | BindingFlags.NonPublic);
        private static RangeRule ConstructRangeRule(RangeValidator validator)
        {
            object minValue = rangeValidatorMinField.GetValue(validator);
            object maxValue = rangeValidatorMaxField.GetValue(validator);
            switch (validator.Type) {
                // RangeValidator's convention is to use type.MinValue/type.MaxValue/type.Empty to
                // signal "no boundary at this end", whereas xVal uses null.
                case RangeValidationType.Integer:
                    var minInt = (int) minValue == int.MinValue ? (int?) null : (int) minValue;
                    var maxInt = (int) maxValue == int.MaxValue ? (int?) null : (int) maxValue;
                    return new RangeRule(minInt, maxInt);
                case RangeValidationType.Decimal:
                    var minDec = (decimal)minValue == decimal.MinValue ? (decimal?)null : (decimal)minValue;
                    var maxDec = (decimal)maxValue == decimal.MaxValue ? (decimal?)null : (decimal)maxValue;
                    return new RangeRule(minDec, maxDec);
                case RangeValidationType.DateTime:
                    var minDateTime = (DateTime)minValue == DateTime.MinValue ? (DateTime?)null : (DateTime)minValue;
                    var maxDateTime = (DateTime)maxValue == DateTime.MaxValue ? (DateTime?)null : (DateTime)maxValue;
                    return new RangeRule(minDateTime, maxDateTime);
                case RangeValidationType.String:
                    var minString = (string)minValue == string.Empty ? null : (string)minValue;
                    var maxString = (string)maxValue == string.Empty ? null : (string)maxValue;
                    return new RangeRule(minString, maxString);
            }
            return null; // Ignore unknown RangeValidationType
        }
    }
}