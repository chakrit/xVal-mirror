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
            EmailAddress
        }
    }
}