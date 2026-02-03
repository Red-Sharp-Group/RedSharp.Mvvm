using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Components;
using RedSharp.Mvvm.Utils;

namespace RedSharp.Mvvm.Abstracts
{
    public abstract class EditableObservableObject : CompositeObservableObject, IEditableObject
    {
        private static readonly TypeRelatedCache<PropertyInfo> _globalCache;

        private EditableObjectComponent _editableObjectComponent;
        
        static EditableObservableObject()
        {
            _globalCache = new TypeRelatedCache<PropertyInfo>();
        }

        public EditableObservableObject()
        {
            _editableObjectComponent = new EditableObjectComponent(this, _globalCache.UnsafeGetParametersForType(GetType()));

            AttachInternalComponent(_editableObjectComponent);
        }

        public bool IsBeingEdited => _editableObjectComponent.IsBeingEdited;

        public void BeginEdit() => _editableObjectComponent.BeginEdit();

        public void CancelEdit() => _editableObjectComponent.CancelEdit();

        public void EndEdit() => _editableObjectComponent.EndEdit();


        protected static void BackupAllProperties<TType>() => BackupAllProperties(typeof(TType));

        protected static void BackupAllProperties(Type type)
        {
            ArgumentsGuard.ThrowIfNull(type, nameof(type));

            var typeInfo = type.GetTypeInfo();
            var properties = typeInfo.DeclaredProperties.Where(item => item.CanWrite && item.CanRead)
                                                        .ToArray();

            _globalCache.RegisterParameters(type, properties);
        }

        protected static void BackupProperties<TType>(params string[] properties) => BackupProperties(typeof(TType), properties);

        protected static void BackupProperties(Type type, params string[] properties)
        {
            var buffer = new List<PropertyInfo>();

            foreach (var name in properties)
            {
                var info = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.IgnoreCase);

                if (info == null)
                    continue;

                if (!info.CanWrite || !info.CanRead)
                    continue;

                buffer.Add(info);
            }

            if (buffer.Count > 0)
                _globalCache.RegisterParameters(type, buffer.ToArray());
        }
    }
}
