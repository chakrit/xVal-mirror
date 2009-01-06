using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace xVal.Rules
{
    public abstract class RuleBase
    {
        public string RuleName { get; private set; }
        private string errorMessage;
        public string ErrorMessage
        {
            get { return errorMessage; }
            set
            {
                if((errorMessageResourceType != null) || (errorMessageResourceName != null))
                    throw new InvalidOperationException("Can't set ErrorMessage: this RuleBase is already in resource mode");
                errorMessage = value;
            }
        }

        private Type errorMessageResourceType;
        public Type ErrorMessageResourceType
        {
            get { return errorMessageResourceType; }
            set
            {
                if(errorMessage != null)
                    throw new InvalidOperationException("Can't set ErrorMessageResourceType: this RuleBase is already in fixed-string mode");
                errorMessageResourceType = value;
            }
        }

        private string errorMessageResourceName;
        public string ErrorMessageResourceName
        {
            get { return errorMessageResourceName; }
            set
            {
                if (errorMessage != null)
                    throw new InvalidOperationException("Can't set ErrorMessageResourceName: this RuleBase is already in fixed-string mode");
                errorMessageResourceName = value;
            }
        }

        protected RuleBase(string ruleName)
        {
            RuleName = ruleName;
        }

        public virtual IDictionary<string, string> ListParameters()
        {
            return new Dictionary<string, string>();
        }
    }
}