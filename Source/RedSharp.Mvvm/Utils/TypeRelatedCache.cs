using System;
using System.Collections.Generic;
using RedSharp.General.Helpers;

namespace RedSharp.Mvvm.Utils
{
    public class TypeRelatedCache<TParameter>
    {
        private Dictionary<Type, HashSet<TParameter>> _storage;

        public TypeRelatedCache()
        {
            _storage = new Dictionary<Type, HashSet<TParameter>>();
        }

        public virtual void RegisterParameters(Type type, params TParameter[] parameter)
        {
            ArgumentsGuard.ThrowIfNull(type, nameof(type));
            ArgumentsGuard.ThrowIfNullOrEmpty(parameter, nameof(parameter));

            var cache = GetCacheForType(type);

            foreach (var item in parameter)
                cache.Add(item);
        }

        public virtual ISet<TParameter> UnsafeGetParametersForType(Type type)
        {
            ArgumentsGuard.ThrowIfNull(type, nameof(type));

            return GetCacheForType(type);
        }

        protected HashSet<TParameter> GetCacheForType(Type type)
        {
            if (!_storage.TryGetValue(type, out var result))
            {
                result = new HashSet<TParameter>();

                var baseType = type.BaseType;

                while (baseType != null)
                {
                    if (_storage.TryGetValue(baseType, out var intermediate))
                        foreach (var key in intermediate)
                            result.Add(key);

                    baseType = baseType.BaseType;
                }

                _storage[type] = result;
            }

            return result;
        }
    }
}
