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

        public override System.Collections.Generic.IDictionary<string, string> ListParameters()
        {
            var result = base.ListParameters();
            if (Min.HasValue) result.Add("Min", Min.ToString());
            if (Max.HasValue) result.Add("Max", Max.ToString());
            return result;
        }
    }
}