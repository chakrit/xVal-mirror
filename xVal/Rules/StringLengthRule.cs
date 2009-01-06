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
            result.Add("MinLength", MinLength.ToString());
            result.Add("MaxLength", MaxLength.ToString());
            return result;
        }
    }
}