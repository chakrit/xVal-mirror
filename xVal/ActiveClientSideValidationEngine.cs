using xVal.ClientSideValidation;

namespace xVal
{
    public static class ActiveClientSideValidationEngine
    {
        public static IClientSideValidationEngine Engine = new jQueryValidationEngine();
    }
}