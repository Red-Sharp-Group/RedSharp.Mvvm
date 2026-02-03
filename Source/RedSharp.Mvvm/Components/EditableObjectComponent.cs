using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Interfaces;

namespace RedSharp.Mvvm.Components
{
    public class EditableObjectComponent : ObservableObject, IEditableObject, IObservableComponent
    {
        private struct PropertyCache
        {
            public PropertyInfo Property;
            public object Value;
        }

        private bool _isBeingEdited;
        private ObservableObject _owner;
        private List<PropertyCache> _cache;

        public EditableObjectComponent(ObservableObject owner, ISet<string> targetProperties)
        {
            ArgumentsGuard.ThrowIfNull(owner, nameof(owner));
            ArgumentsGuard.ThrowIfNullOrEmpty(targetProperties, nameof(targetProperties));

            _owner = owner;

            var typeInfo = _owner.GetType()
                                 .GetTypeInfo();

            var propertyInfos = typeInfo.DeclaredProperties.Where(item => targetProperties.Contains(item.Name))
                                                           .ToList();

            InitializeCache(propertyInfos);
        }

        public EditableObjectComponent(ObservableObject owner, ISet<PropertyInfo> targetProperties)
        {
            ArgumentsGuard.ThrowIfNull(owner, nameof(owner));
            ArgumentsGuard.ThrowIfNullOrEmpty(targetProperties, nameof(targetProperties));

            _owner = owner;

            InitializeCache(targetProperties);
        }

        public bool IsBeingEdited
        {
            get => _isBeingEdited;
            private set
            {
                if (_isBeingEdited == value)
                    return;

                RaisePropertyChanging();

                _isBeingEdited = value;

                RaisePropertyChanged();
            }
        }

        public void BeginEdit()
        {
            if (IsBeingEdited)
                return;

            for (int i = 0; i < _cache.Count; i++)
            {
                var property = _cache[i].Property;
                var value = property.GetValue(_owner);

                _cache[i] = new PropertyCache()
                {
                    Property = property,
                    Value = value
                };
            }

            IsBeingEdited = true;
        }

        public void CancelEdit()
        {
            if (!IsBeingEdited)
                return;

            for (int i = 0; i < _cache.Count; i++)
            {
                var property = _cache[i].Property;
                var value = _cache[i].Value;

                property.SetValue(_owner, value);

                _cache[i] = new PropertyCache()
                {
                    Property = property,
                    Value = null
                };
            }

            IsBeingEdited = false;
        }

        public void EndEdit()
        {
            if (!IsBeingEdited)
                return;

            for (int i = 0; i < _cache.Count; i++)
            {
                var property = _cache[i].Property;

                _cache[i] = new PropertyCache()
                {
                    Property = property,
                    Value = null
                };
            }

            IsBeingEdited = false;
        }

        void IObservableComponent.OwnerPropertyChanged<TValue>(TValue value, string propertyName)
        { }

        private void InitializeCache(ICollection<PropertyInfo> properties)
        {
            _cache = new List<PropertyCache>(properties.Count);

            foreach (var item in properties)
            {
                if (!item.CanWrite || !item.CanRead)
                    continue;

                _cache.Add(new PropertyCache()
                {
                    Property = item,
                    Value = null
                });
            }
        }
    }
}
