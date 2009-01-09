using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Linq;

namespace xVal.ServerSide
{
    public class RulesException
    {
        public RulesException(IEnumerable<ErrorInfo> errors)
        {
            Errors = errors;
        }

        public RulesException(string propertyName, string errorMessage)
            : this(propertyName, errorMessage, null) {}

        public RulesException(string propertyName, string errorMessage, object onObject)
        {
            Errors = new[] { new ErrorInfo(propertyName, errorMessage, onObject) };
        }

        public IEnumerable<ErrorInfo> Errors { get; private set; }

        public void PopulateModelState(ModelStateDictionary modelState, string prefix)
        {
            PopulateModelState(modelState, prefix, x => true);
        }

        public void PopulateModelState(ModelStateDictionary modelState, string prefix, Func<ErrorInfo, bool> errorFilter)
        {
            if (errorFilter == null) throw new ArgumentNullException("errorFilter");
            prefix = prefix == null ? "" : prefix + ".";
            foreach (var errorInfo in Errors.Where(errorFilter)) {
                modelState.AddModelError(prefix + errorInfo.PropertyName, errorInfo.ErrorMessage);
            }
        }
    }
}