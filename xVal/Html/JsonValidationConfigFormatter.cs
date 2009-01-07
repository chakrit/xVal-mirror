using System;
using System.Collections;
using System.Collections.Generic;
using System.Web.Routing;
using System.Web.Script.Serialization;
using xVal.RuleProviders;
using System.Linq;
using xVal.Rules;

namespace xVal.Html
{
    public class JsonValidationConfigFormatter : IValidationConfigFormatter
    {
        static readonly JavaScriptSerializer Serializer = new JavaScriptSerializer();
        static JsonValidationConfigFormatter()
        {
            Serializer.RegisterConverters(new JavaScriptConverter[] { new RuleSetConverter(), new RuleBaseConverter() });
        }

        public string FormatRules(RuleSet rules)
        {
            return Serializer.Serialize(rules);
        }

        private class RuleBaseConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new System.NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                var ruleBase = obj as RuleBase;
                if (ruleBase == null)
                    throw new ArgumentException("obj must be of type RouteBase");

                return new Dictionary<string, object> {
                    { "RuleName", ruleBase.RuleName },
                    { "RuleParameters", ruleBase.ListParameters() }
                };
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new[] { typeof(RuleBase) }; }
            }
        }

        private class RuleSetConverter : JavaScriptConverter
        {
            public override object Deserialize(IDictionary<string, object> dictionary, Type type, JavaScriptSerializer serializer)
            {
                throw new System.NotImplementedException();
            }

            public override IDictionary<string, object> Serialize(object obj, JavaScriptSerializer serializer)
            {
                var ruleSet = obj as RuleSet;
                if(ruleSet == null)
                    throw new ArgumentException("obj must be of type RuleSet");
                return new Dictionary<string, object> {
                    { "Fields", ruleSet.Keys.Select(x => new { FieldName = x, FieldRules = ruleSet[x].ToArray() }).ToArray()}
                };
                //return ruleSet.Keys.ToDictionary(x => x, x => (object)ruleSet[x]);
            }

            public override IEnumerable<Type> SupportedTypes
            {
                get { return new[] { typeof(RuleSet) }; }
            }
        }


    }
}