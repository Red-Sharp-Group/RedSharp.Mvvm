using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Interfaces;

namespace RedSharp.Mvvm.Abstracts
{
    public abstract class CompositeObservableObject : ObservableObject
    {
        private IObservableComponent[] _components;
        private int _componentsNumber;

        protected IReadOnlyList<IObservableComponent> EnumerateInternalComponents()
        {
            if (_components == null)
                return Array.Empty<IObservableComponent>();

            var result = new IObservableComponent[_componentsNumber];

            Array.Copy(_components, result, _componentsNumber);

            return result;
        }

        protected void AttachInternalComponent<TComponent>() where TComponent : IObservableComponent, new()
        {
            AttachInternalComponent(new TComponent());
        }

        protected void AttachInternalComponent(IObservableComponent component)
        {
            if (_components == null)
            {
                _components = new IObservableComponent[1] { component };
                _componentsNumber = 1;
            }
            else 
            {
                if (_componentsNumber == _components.Length)
                {
                    var newArray = new IObservableComponent[_components.Length * 2];

                    Array.Copy(_components, newArray, _components.Length);

                    _components = newArray;
                }

                _components[_componentsNumber] = component;
                _componentsNumber++;
            }

            component.PropertyChanging += OnComponentPropertyChanging;
            component.PropertyChanged += OnComponentPropertyChanged;
        }

        private void OnComponentPropertyChanging(object sender, PropertyChangingEventArgs arguments) => RaisePropertyChanging(arguments);

        private void OnComponentPropertyChanged(object sender, PropertyChangedEventArgs arguments) => RaisePropertyChanged(arguments);

        protected override void SetValue<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            base.SetValue(ref field, value, propertyName);

            NotifyComponents(value, propertyName);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected void NotifyComponents<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            ArgumentsGuard.ThrowIfNull(propertyName, nameof(propertyName));

            if (_componentsNumber == 0)
                return;

            for (int i = 0; i < _componentsNumber && i < _components.Length; i++)
                _components[i].OwnerPropertyChanged(value, propertyName);
        }        
    }
}
