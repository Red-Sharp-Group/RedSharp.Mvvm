using System;
using System.Collections.Generic;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Utils
{
    public class ValidationRegistry
    {
        private Dictionary<Type, Dictionary<string, List<IValidationRule>>> _validationRules;

        public ValidationRegistry() 
        {
            _validationRules = new Dictionary<Type, Dictionary<string, List<IValidationRule>>>();
        }

        /// <summary>
        /// Register the validation rule for the every input <paramref name="properties"/> of the <paramref name="type"/>
        /// </summary>
        /// <remarks>
        /// The existing of the properties is not checked
        /// </remarks>
        public void RegisterValidationRule(Type type, IValidationRule ValidationRule, string[] properties)
        {
            var rules = GetOrCreateValidationRulesForType(type);

            foreach (var property in properties)
            {
                if (!rules.TryGetValue(property, out var list))
                {
                    list = new List<IValidationRule>();

                    rules.Add(property, list);
                }

                list.Add(ValidationRule);
            }
        }

        public ValidationEngine CreateValidationEngine(Type type)
        {
            var validatorType = typeof(ObservableObject);

            while (type != null && type != validatorType)
            {
                if (_validationRules.TryGetValue(type, out var cache))
                    return new ValidationEngine(cache);

                type = type.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Returns collection of existing rules or creates a new one inheriting every registered rule from base types
        /// </summary>
        /// <remarks>
        /// If new rule will be added in base type it will not appear in the inherited collection - this is a TODO limitation
        /// </remarks>
        private Dictionary<string, List<IValidationRule>> GetOrCreateValidationRulesForType(Type type)
        {
            if (_validationRules.TryGetValue(type, out var result))
                return result;

            result = new Dictionary<string, List<IValidationRule>>(StringComparer.InvariantCultureIgnoreCase);

            var defaultType = typeof(object);
            var baseType = type.BaseType;

            while (baseType != null && baseType != defaultType)
            {
                if (_validationRules.TryGetValue(baseType, out var baseTypeCache))
                {
                    foreach (var item in baseTypeCache)
                    {
                        if (!result.TryGetValue(item.Key, out var list))
                        {
                            list = new List<IValidationRule>();

                            result.Add(item.Key, list);
                        }

                        list.AddRange(item.Value);
                    }
                }

                baseType = baseType.BaseType;
            }

            _validationRules[type] = result;

            return result;
        }
    }
}
