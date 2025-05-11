using System;
using System.Collections.Generic;
using System.ComponentModel;
using RedSharp.Mvvm.Bindings.Abstracts;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{
    /// <summary>
    /// The chain that operates with observable objects
    /// </summary>
    public class NotifyPropertyChangedChain<TTarget, TValue> : BindingChainBase<TValue> where TTarget : INotifyPropertyChanged
    {
        private TTarget _target;
        private string _name;

        private Func<TTarget, TValue> _getter;
        private Action<TTarget, TValue> _setter;

        public NotifyPropertyChangedChain(string name, Func<TTarget, TValue> getter, Action<TTarget, TValue> setter = null)
        {
            _name = name;
            _getter = getter;
            _setter = setter;
        }

        /// <inheritdoc/>
        public override bool IsReadOnly => _setter == null;

        /// <inheritdoc/>
        public override TValue Value
        {
            get
            {
                if (_target != null)
                    return _getter.Invoke(_target);

                return default;
            }
            set
            {
                ThrowIfReadOnly();

                if (_target != null)
                    _setter(_target, value);
            }
        }

        /// <inheritdoc/>
        public override bool TryUpdateTarget(IBindingChain previousChain)
        {
            var previousTarget = _target;

            if (_target != null)
                _target.PropertyChanged -= OnTargetPropertyChanged;

            if (previousChain != null && previousChain.Value is TTarget castedTarget)
                _target = castedTarget;
            else
                _target = default;

            if (_target != null)
                _target.PropertyChanged += OnTargetPropertyChanged;

            return !EqualityComparer<TTarget>.Default.Equals(_target, previousTarget);
        }

        private void OnTargetPropertyChanged(object sender, PropertyChangedEventArgs arguments)
        {
            if (arguments.PropertyName == _name)
                RaiseValueChanged();
        }
    }
}
