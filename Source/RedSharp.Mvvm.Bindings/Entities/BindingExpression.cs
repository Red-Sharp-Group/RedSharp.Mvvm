using System.Collections.Generic;
using System.ComponentModel;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Bindings.Abstracts;
using RedSharp.Mvvm.Bindings.Interfaces;
using RedSharp.Mvvm.Interfaces;

namespace RedSharp.Mvvm.Bindings.Entities
{

    /// <summary>
    /// The default implementation of the <see cref="IBindingExpression{TValue}"/> interfaces
    /// </summary>
    public class BindingExpression<TValue> : BindingExpressionBase<TValue>
    {
        public static readonly PropertyChangingEventArgs ValueChanging = new PropertyChangingEventArgs(nameof(Value));

        public static readonly PropertyChangedEventArgs ValueChanged = new PropertyChangedEventArgs(nameof(Value));

        public BindingExpression(IReadOnlyCollection<IBindingChain> chains) : base(chains)
        {
            ForceUpdate();
        }

        protected override void OnFinalChainChanging() => RaisePropertyChanging(ValueChanging);

        protected override void OnFinalChainChanged() => RaisePropertyChanged(ValueChanged);
    }
}
