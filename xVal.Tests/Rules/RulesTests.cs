using Xunit;
using xVal.Rules;

namespace xVal.Tests.Rules
{
    public class RulesTests
    {
        [Fact]
        public void RequiredRule_Needs_No_Parameters()
        {
            var rule = new RequiredRule();
            Assert.Empty(rule.ListParameters());
        }

        [Fact]
        public void StringLengthRule_Takes_Min_And_Max_Lengths()
        {
            var rule = new StringLengthRule(1, 5);
            var parameters = rule.ListParameters();
            Assert.Equal(2, parameters.Count);
            Assert.Equal("1", parameters["MinLength"]);
            Assert.Equal("5", parameters["MaxLength"]);
        }

        [Fact]
        public void NumericRangeRule_Take_Min_And_Max()
        {
            var rule = new NumericRangeRule(1, 5);
            var parameters = rule.ListParameters();
            Assert.Equal(2, parameters.Count);
            Assert.Equal("1", parameters["Min"]);
            Assert.Equal("5", parameters["Max"]);
        }
    }
}