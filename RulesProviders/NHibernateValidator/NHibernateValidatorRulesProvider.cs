using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using NHibernate.Validator;
using NHibernate.Validator.Cfg;
using NHibernate.Validator.Cfg.MappingSchema;
using NHibernate.Validator.Engine;
using NHibernate.Validator.Mappings;
using xVal.RuleProviders;
using xVal.Rules;

namespace xVal.RulesProviders.NHibernateValidator
{
    /// <summary>
    /// Note: This provider is incomplete and is not yet intended to be used.
    /// Ideally it would be in a separate branch at the moment, but I don't think that's possible.
    /// </summary>
    [Obsolete("Don't use NHibernateValidatorRulesProvider yet - it's incomplete.")]
    public class NHibernateValidatorRulesProvider : CachingRulesProvider
    {
        private readonly ValidatorMode configMode;
        private readonly RuleEmitterList<IRuleArgs> ruleEmitters = new RuleEmitterList<IRuleArgs>();

        public NHibernateValidatorRulesProvider(ValidatorMode configMode)
        {
            this.configMode = configMode;

            ruleEmitters.AddSingle<LengthAttribute>(ConvertLengthAttributeToStringLengthRule);
        }

        protected override RuleSet GetRulesFromTypeCore(Type type)
        {
            var classMapping = ClassMappingFactory.GetClassMapping(type, configMode);

            var rules = from member in type.GetMembers()
                        where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
                        from att in classMapping.GetMemberAttributes(member).OfType<IRuleArgs>() // All NHibernate Validation validators attributes must implement this interface
                        from rule in ConvertToXValRules(att)
                        where rule != null
                        select new { MemberName = member.Name, Rule = rule };

            return new RuleSet(rules.ToLookup(x => x.MemberName, x => x.Rule));
        }

        private IEnumerable<RuleBase> ConvertToXValRules(IRuleArgs att)
        {
            foreach (var rule in ruleEmitters.EmitRules(att)) {
                if(rule != null) {
                    rule.ErrorMessage = att.Message;
                    yield return rule;
                }
            }
        }

        private static StringLengthRule ConvertLengthAttributeToStringLengthRule(LengthAttribute att)
        {
            return new StringLengthRule(att.Min, att.Max);
        }

        private static class ClassMappingFactory
        {
            public static IClassMapping GetClassMapping(Type type, ValidatorMode mode)
            {
                IClassMapping result = null;
                switch (mode) {
                    case ValidatorMode.UseAttribute:
                        break;
                    case ValidatorMode.UseXml:
                        result = new XmlClassMapping(GetXmlDefinitionFor(type));
                        break;
                    case ValidatorMode.OverrideAttributeWithXml:
                        var xmlDefinition = GetXmlDefinitionFor(type);
                        if (xmlDefinition != null)
                            result = new XmlOverAttributeClassMapping(xmlDefinition);
                        break;
                    case ValidatorMode.OverrideXmlWithAttribute:
                        var xmlDefinition2 = GetXmlDefinitionFor(type);
                        if (xmlDefinition2 != null)
                            result = new AttributeOverXmlClassMapping(xmlDefinition2);
                        break;
                }
                return result ?? new ReflectionClassMapping(type);
            }

            private static NhvmClass GetXmlDefinitionFor(Type type)
            {
                var mapp = MappingLoader.GetMappingFor(type);
                return mapp != null && mapp.@class.Length > 0 ? mapp.@class[0] : null;
            }
        }
    }
}