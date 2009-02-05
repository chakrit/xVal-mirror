using System;
using System.Collections.Generic;
using xVal.Rules;
using System.Linq;

namespace xVal.RuleProviders
{
    // Note: RuleEmitters should return null if they don't want to match the input
    public class RuleEmitterList<TInputBase>
    {
        public delegate IEnumerable<RuleBase> RuleEmitter(TInputBase item);

        private readonly List<RuleEmitter> ruleEmitters = new List<RuleEmitter>();

        public void AddSingle<TSource>(Func<TSource, RuleBase> emitter) where TSource : TInputBase
        {
            ruleEmitters.Add(x => {
                if (x is TSource) {
                    RuleBase rule = emitter((TSource)x);
                    return rule == null ? null : new[] {rule};
                } else {
                    return null;
                }
            });
        }

        public void AddMultiple<TSource>(RuleEmitter emitter) where TSource : TInputBase
        {
            ruleEmitters.Add(x => (x is TSource) ? emitter((TSource)x) : null);
        }

        public IEnumerable<RuleBase> EmitRules(TInputBase item)
        {
            foreach (var converter in ruleEmitters) {
                var converterResult = converter(item);
                if (converterResult != null)
                    return converterResult;
            }
            return new RuleBase[] {}; // No matching converter, so return empty set of rules
        }
    }
}