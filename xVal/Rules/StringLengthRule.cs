namespace xVal.Rules
{
    public class StringLengthRule : RuleBase
    {
        public int? MinLength { get; private set; }
        public int? MaxLength { get; private set; }

        public StringLengthRule(int? minLength, int? maxLength) : base("StringLength")
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public override System.Collections.Generic.IDictionary<string, string> ListParameters()
        {
            var result = base.ListParameters();
            if(MinLength.HasValue) result.Add("MinLength", MinLength.ToString());
            if(MaxLength.HasValue) result.Add("MaxLength", MaxLength.ToString());
            return result;
        }
    }
}