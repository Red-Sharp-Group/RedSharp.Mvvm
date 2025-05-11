using System;
using System.Collections.Generic;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Utils
{
    public class GlobalValidationCache
    {
        private Dictionary<string, Dictionary<string, List<IValidationRule>>> _validationRules;

        public GlobalValidationCache() 
        {
            _validationRules = new Dictionary<string, Dictionary<string, List<IValidationRule>>>();
        }

        public void RegisterValidationRule(Type type, IValidationRule ValidationRule, string[] properties)
        {
            var localValidationCache = GetOrCreateLocalValidationCache(type);

            foreach (var property in properties)
            {
                if (!localValidationCache.TryGetValue(property, out var list))
                {
                    list = new List<IValidationRule>();

                    localValidationCache.Add(property, list);
                }

                list.Add(ValidationRule);
            }
        }

        public LocalValidationCache CreateLocalCache(Type type)
        {
            var validatorType = typeof(ObservableObject);

            while (type != null && type != validatorType)
            {
                if (_validationRules.TryGetValue(type.FullName, out var cache))
                    return new LocalValidationCache(cache);

                type = type.BaseType;
            }

            return null;
        }

        private Dictionary<string, List<IValidationRule>> GetOrCreateLocalValidationCache(Type type)
        {
            if (_validationRules.TryGetValue(type.FullName, out var cache))
                return cache;

            cache = new Dictionary<string, List<IValidationRule>>();

            var defaultType = typeof(ObservableObject);
            var baseType = type.BaseType;

            while (baseType != null && baseType != defaultType)
            {
                if (_validationRules.TryGetValue(baseType.FullName, out var baseTypeCache))
                {
                    foreach (var item in baseTypeCache)
                    {
                        if (!cache.TryGetValue(item.Key, out var list))
                        {
                            list = new List<IValidationRule>();

                            cache.Add(item.Key, list);
                        }

                        list.AddRange(item.Value);
                    }
                }

                baseType = baseType.BaseType;
            }

            _validationRules[type.FullName] = cache;

            return cache;
        }
    }
}
