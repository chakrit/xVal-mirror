namespace xVal.Rules
{
    public class DataTypeRule : RuleBase
    {
        public DataType Type { get; private set; }

        public DataTypeRule(DataType dataType) : base("DataType")
        {
            Type = dataType;
        }

        public enum DataType
        {
            Integer,
            Decimal,
            Date,
            DateTime,
            Currency,
            EmailAddress,
            CreditCardLuhn
        }

        public override System.Collections.Generic.IDictionary<string, string> ListParameters()
        {
            var result = base.ListParameters();
            result.Add("Type", Type.ToString());
            return result;
        }
    }
}