namespace xVal.Rules
{
    public class NumericRangeRule : RuleBase
    {
        public decimal? Min { get; set; }
        public decimal? Max { get; set; }

        public NumericRangeRule(decimal? min, decimal? max) : base("NumericRange")
        {
            Min = min;
            Max = max;
        }
    }
}