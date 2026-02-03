using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Components;
using RedSharp.Mvvm.Utils;

namespace RedSharp.Mvvm.Abstracts
{
    public abstract class ChangeTrackingObservableObject : CompositeObservableObject, IChangeTracking
    {
        private static readonly TypeRelatedCache<string> _globalCache;

        private ChangeTrackingComponent _trackingComponent;

        static ChangeTrackingObservableObject()
        {
            _globalCache = new TypeRelatedCache<string>();
        }

        public ChangeTrackingObservableObject()
        {
            _trackingComponent = new ChangeTrackingComponent(_globalCache.UnsafeGetParametersForType(GetType()));

            AttachInternalComponent(_trackingComponent);
        }

        public bool IsChanged => _trackingComponent.IsChanged;

        public void AcceptChanges() => _trackingComponent.AcceptChanges();


        protected static void TrackAllProperties<TType>() => TrackAllProperties(typeof(TType));

        protected static void TrackAllProperties(Type type)
        {
            ArgumentsGuard.ThrowIfNull(type, nameof(type));

            var typeInfo = type.GetTypeInfo();
            var properties = typeInfo.DeclaredProperties.Select(item => item.Name)
                                                        .ToArray();

            TrackProperties(type, properties);
        }

        protected static void TrackProperties<TType>(params string[] properties) => TrackProperties(typeof(TType), properties);

        protected static void TrackProperties(Type type, params string[] properties) => _globalCache.RegisterParameters(type, properties);
    }
}
