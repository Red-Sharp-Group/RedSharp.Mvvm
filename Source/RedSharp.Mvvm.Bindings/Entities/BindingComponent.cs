using System.Collections.Generic;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Bindings.Abstracts;
using RedSharp.Mvvm.Bindings.Interfaces;
using RedSharp.Mvvm.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{
    public class BindingComponent<TValue> : BindingExpressionBase<TValue>, IObservableComponent
    {
        private string _propertyName;

        public BindingComponent(IReadOnlyCollection<IBindingChain> chains, string propertyName) : base(chains)
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(propertyName, nameof(propertyName));

            _propertyName = propertyName;

            ForceUpdate();
        }

        public void OwnerPropertyChanged<TValue1>(TValue1 value, string propertyName)
        { }

        protected override void OnFinalChainChanging() => RaisePropertyChanging(_propertyName);

        protected override void OnFinalChainChanged() => RaisePropertyChanged(_propertyName);
    }
}
