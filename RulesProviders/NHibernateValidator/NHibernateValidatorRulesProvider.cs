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
    public class NHibernateValidatorRulesProvider : IRulesProvider
    {
        private readonly ValidatorMode configMode;
        private readonly IDictionary<Type, Func<object, RuleBase>> converters = new Dictionary<Type, Func<object, RuleBase>>();

        public NHibernateValidatorRulesProvider(ValidatorMode configMode)
        {
            this.configMode = configMode;

            RegisterConverter<LengthAttribute>(ConvertLengthAttributeToStringLengthRule);
        }

        public void RegisterConverter<TAttribute>(Func<TAttribute, RuleBase> converter) where TAttribute: Attribute
        {
            Func<object, RuleBase> objectConverter = x => converter((TAttribute) x);
            if (converters.ContainsKey(typeof(TAttribute)))
                converters[typeof (TAttribute)] = objectConverter;
            else
                converters.Add(typeof(TAttribute), objectConverter);
        }

        public RuleSet GetRulesFromType(Type type)
        {
            var classMapping = ClassMappingFactory.GetClassMapping(type, configMode);

            var rules = from member in type.GetMembers()
                        where member.MemberType == MemberTypes.Field || member.MemberType == MemberTypes.Property
                        from att in classMapping.GetMemberAttributes(member)
                        where typeof(IRuleArgs).IsAssignableFrom(att.GetType()) // All NHibernate Validation validators attributes must implement this interface
                        let rule = ConvertToXValRule(att)
                        where rule != null
                        select new { MemberName = member.Name, Rule = rule };

            return new RuleSet(rules.ToLookup(x => x.MemberName, x => x.Rule));
        }

        private RuleBase ConvertToXValRule(Attribute att)
        {
            RuleBase result = null;
            
            if(converters.ContainsKey(att.GetType()))
                result = converters[att.GetType()](att);

            if (result != null)
                result.ErrorMessage = ((IRuleArgs) att).Message;

            return result;
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