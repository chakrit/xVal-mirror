using System.Collections.Generic;
using System.Web.Routing;
using Xunit;
using xVal.Html;
using xVal.RuleProviders;
using xVal.Rules;
using System.Linq;

namespace xVal.Tests.HtmlHelpers
{
    public class DefaultValidationConfigFormatterTests
    {
        [Fact]
        public void Empty_Ruleset_Formatted_With_Name()
        {
            // Arrange
            var formatter = new DefaultValidationConfigFormatter();

            // Act
            var result = formatter.FormatRules(RuleSet.Empty, "myname");

            // Assert
            Assert.Equal("<ruleset name=\"myname\" />", result);
        }

        [Fact]
        public void Single_Rule()
        {
            // Arrange
            var formatter = new DefaultValidationConfigFormatter();
            var rules = MakeTestRuleSet(new Dictionary<string, IDictionary<string, object>> {
                { 
                    "myprop", new Dictionary<string, object> {
                        { "somerule", new { param1 = "param1value", param2 = "param2value" } }    
                    } 
                }
            });

            // Act
            var result = formatter.FormatRules(rules, "myname");

            // Assert
            Assert.Equal(@"<ruleset name=""myname"">
    <somerule forfield=""myprop"" param1=""param1value"" param2=""param2value"" />
</ruleset>", result);
        }

        [Fact]
        public void Multiple_Rules()
        {
            // Arrange
            var formatter = new DefaultValidationConfigFormatter();
            var rules = MakeTestRuleSet(new Dictionary<string, IDictionary<string, object>> {
                { 
                    "screenplay", new Dictionary<string, object> {
                        { "copyright", new { author = "Wm. Shakespeare", year = "1668" } },
                        { "description", new { language = "Welsh", grammar = "perfect", length = "long" } }    
                    }
                },
                {
                    "petname", new Dictionary<string, object> {
                        { "required", new { } },
                        { "DataType", new { @type = "EmailAddress", domainSuffix = ".co.uk" } },
                        { "LengthConstraint", new { max = "150" } }
                    }
                }
            });

            // Act
            var result = formatter.FormatRules(rules, "somerules");

            // Assert
            // Notice that the attributes are re-ordered (into alphabetical order)
            // This test code is a bit flaky because the attributes could be ordered differently in future
            // Not sure if there's a better way to test this (maybe will parse the output as XML then inspect independently of order)
            Assert.Equal(@"<ruleset name=""somerules"">
    <copyright author=""Wm. Shakespeare"" forfield=""screenplay"" year=""1668"" />
    <description forfield=""screenplay"" grammar=""perfect"" language=""Welsh"" length=""long"" />
    <required forfield=""petname"" />
    <DataType domainSuffix="".co.uk"" forfield=""petname"" type=""EmailAddress"" />
    <LengthConstraint forfield=""petname"" max=""150"" />
</ruleset>", result);
        }

        // Utility function to give a quick syntax for instantiating a complete RuleSet
        private RuleSet MakeTestRuleSet(IDictionary<string, IDictionary<string, object>> data)
        {
            var rules = from propName in data.Keys
                        from ruleName in data[propName].Keys
                        select new { propName, rule = (RuleBase) new TestRule(ruleName, data[propName][ruleName]) };
            return new RuleSet(rules.ToLookup(x => x.propName, x => x.rule));
        }

        private class TestRule : RuleBase
        {
            public IDictionary<string, string> Parameters { get; set; }

            public TestRule(string ruleName, object parameters) : base(ruleName)
            {
                Parameters = new RouteValueDictionary(parameters).ToDictionary(x => x.Key, x => x.Value.ToString());
            }

            public override IDictionary<string, string> ListParameters()
            {
                return Parameters;
            }
        }
    }
}