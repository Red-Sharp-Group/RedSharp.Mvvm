using System;
using System.Collections.Generic;
using System.ComponentModel;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Bindings.Entities;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Helpers
{
    /// <summary>
    /// Simplifies the process of binding creation
    /// </summary>
    public static class BindingBuilderHelper
    {
        public struct BindingBuilder<TValue>
        {
            internal List<IBindingChain> Bindings;

            internal IBindingChain<TValue> LastChain;
        }

        /// <summary>
        /// Starts the binding builder by creating the first two chains with the observable object
        /// </summary>
        public static BindingBuilder<TValue> Bind<TInput, TValue>(this TInput input,
                                                                  string name,
                                                                  Func<TInput, TValue> getter,
                                                                  Action<TInput, TValue> setter = null) where TInput : INotifyPropertyChanged
        {
            ArgumentsGuard.ThrowIfNull(input, nameof(input));

            var fixedValueChain = new FixedValueChain<TInput>();
            var notifyPropertyChangedChain = new NotifyPropertyChangedChain<TInput, TValue>(name, getter, setter);

            fixedValueChain.ForceUpdateTarget(input);

            return new BindingBuilder<TValue>
            {
                Bindings = new List<IBindingChain> { fixedValueChain, notifyPropertyChangedChain },
                LastChain = notifyPropertyChangedChain
            };
        }

        /// <summary>
        /// Continues binding expression with the observable object
        /// </summary>
        public static BindingBuilder<TValue> Bind<TInput, TValue>(this BindingBuilder<TInput> builder,
                                                                  string name,
                                                                  Func<TInput, TValue> getter,
                                                                  Action<TInput, TValue> setter = null) where TInput : INotifyPropertyChanged
        {
            var notifyPropertyChangedChain = new NotifyPropertyChangedChain<TInput, TValue>(name, getter, setter);

            builder.Bindings.Add(notifyPropertyChangedChain);

            return new BindingBuilder<TValue>
            {
                Bindings = builder.Bindings,
                LastChain = notifyPropertyChangedChain
            };
        }

        /// <summary>
        /// Continues binding expression with value converting
        /// </summary>
        public static BindingBuilder<TOutput> Convert<TInput, TOutput>(this BindingBuilder<TInput> builder, Func<TInput, TOutput> converter)
        {
            var converterChain = new ConverterChain<TInput, TOutput>(converter);

            builder.Bindings.Add(converterChain);

            return new BindingBuilder<TOutput>
            {
                Bindings = builder.Bindings,
                LastChain = converterChain
            };
        }

        /// <summary>
        /// Completes binding expression building and returns fully formed expression
        /// </summary>
        public static IBindingExpression<TValue> Complete<TValue>(this BindingBuilder<TValue> builder)
        {
            return new BindingExpression<TValue>(builder.Bindings);
        }
    }
}
