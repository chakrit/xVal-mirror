using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using xVal.RuleProviders;
using xVal.Rules;
using System.Linq;

namespace xVal.ClientSidePlugins.TestHelpers
{
    public static class SampleRuleSets
    {
        public static RuleSet Person
        {
            get
            {
                var rules = new List<KeyValuePair<string, RuleBase>> {
                    new KeyValuePair<string, RuleBase>("Name", new RequiredRule { ErrorMessage = "State your name"}),
                    new KeyValuePair<string, RuleBase>("Age", new RangeRule(0, 150) { ErrorMessage = "Age must be within range 0-150"})
                };
                return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
            }
        }

        public static RuleSet AllPossibleRules
        {
            get
            {
                var rules = new List<KeyValuePair<string, RuleBase>> {
                    new KeyValuePair<string, RuleBase>("RequiredField", new RequiredRule { ErrorMessage = "This is a custom error message for a required field"}),
                    new KeyValuePair<string, RuleBase>("DataType_EmailAddress_Field", new DataTypeRule(DataTypeRule.DataType.EmailAddress)),
                    new KeyValuePair<string, RuleBase>("DataType_CreditCardLuhn_Field", new DataTypeRule(DataTypeRule.DataType.CreditCardLuhn)),
                    new KeyValuePair<string, RuleBase>("DataType_Integer_Field", new DataTypeRule(DataTypeRule.DataType.Integer)),
                    new KeyValuePair<string, RuleBase>("DataType_Decimal_Field", new DataTypeRule(DataTypeRule.DataType.Decimal)),
                    new KeyValuePair<string, RuleBase>("DataType_Date_Field", new DataTypeRule(DataTypeRule.DataType.Date)),
                    new KeyValuePair<string, RuleBase>("DataType_DateTime_Field", new DataTypeRule(DataTypeRule.DataType.DateTime)),
                    new KeyValuePair<string, RuleBase>("DataType_Currency_Field", new DataTypeRule(DataTypeRule.DataType.Currency)),
                    new KeyValuePair<string, RuleBase>("Regex_Field", new RegularExpressionRule("[A-Z]\\d{3}") { ErrorMessage = "Enter a value of the form 'X123'"}),
                    new KeyValuePair<string, RuleBase>("Regex_CaseInsensitive_Field", new RegularExpressionRule("[A-Z]{3}", RegexOptions.IgnoreCase) { ErrorMessage = "Enter a value of the form 'aBc'"}),
                    new KeyValuePair<string, RuleBase>("Range_Int_Field", new RangeRule((int)5, (int)10)),
                    new KeyValuePair<string, RuleBase>("Range_Decimal_Field", new RangeRule(null, 10.98m)),
                    new KeyValuePair<string, RuleBase>("Range_String_Field", new RangeRule("aardvark", "antelope")),
                    new KeyValuePair<string, RuleBase>("Range_DateTime_Field", new RangeRule(new DateTime(2001, 2, 19, 17, 04, 59), (DateTime?)null)),
                    new KeyValuePair<string, RuleBase>("StringLength_Min_Field", new StringLengthRule(3, null)),
                    new KeyValuePair<string, RuleBase>("StringLength_Max_Field", new StringLengthRule(null, 6)),
                    new KeyValuePair<string, RuleBase>("StringLength_Range_Field", new StringLengthRule(4, 7)),
                    new KeyValuePair<string, RuleBase>("Comparison_Equals", new ComparisonRule("RequiredField", ComparisonRule.Operator.Equals)),
                    new KeyValuePair<string, RuleBase>("Comparison_DoesNotEqual", new ComparisonRule("RequiredField", ComparisonRule.Operator.DoesNotEqual)),
                };
                return new RuleSet(rules.ToLookup(x => x.Key, x => x.Value));
            }
        }
    }
}