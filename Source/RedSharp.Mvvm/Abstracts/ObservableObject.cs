using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedSharp.General.Helpers;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// Simple implementation of the <see cref="INotifyPropertyChanging"/> and <see cref="INotifyPropertyChanged"/> events
    /// </summary>
    public abstract class ObservableObject : INotifyPropertyChanging, INotifyPropertyChanged
    {
        /// <inheritdoc/>
        public event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc/>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Should be called before the value is changed
        /// </summary>
        /// <remarks>
        /// Performs call in try catch statement
        /// </remarks>
        protected void RaisePropertyChanging([CallerMemberName] string property = null) => PropertyChanging.InvokeSafe(this, property);

        /// <summary>
        /// Should be called after the value is changed
        /// </summary>
        /// <remarks>
        /// Performs call in try catch statement
        /// </remarks>
        protected void RaisePropertyChanged([CallerMemberName] string property = null) => PropertyChanged.InvokeSafe(this, property);


        /// <summary>
        /// Should be called before the value is changed
        /// </summary>
        /// <remarks>
        /// Performs call in try catch statement.
        /// <br/>This version is for the cases when the property is repetitive so arguments was pre-cached
        /// </remarks>
        protected void RaisePropertyChanging(PropertyChangingEventArgs arguments) => PropertyChanging.InvokeSafe(this, arguments);


        /// <summary>
        /// Should be called after the value is changed
        /// </summary>
        /// <remarks>
        /// Performs call in try catch statement.
        /// <br/>This version is for the cases when the property is repetitive so arguments was pre-cached
        /// </remarks>
        protected void RaisePropertyChanged(PropertyChangedEventArgs arguments) => PropertyChanged.InvokeSafe(this, arguments);

        /// <summary>
        /// Sets the value with calling of corresponding events
        /// </summary>
        /// <remarks>
        /// Does not provide equality check.
        /// <br/>Can be overridden for cases like a validation providing
        /// </remarks>
        protected virtual void SetValue<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            RaisePropertyChanging(propertyName);

            field = value;

            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Special version of "compare and set" for strings
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CompareAndSetStringValue(ref string field, string value, StringComparison comparison = StringComparison.Ordinal, [CallerMemberName] string propertyName = null)
        {
            if (string.Equals(field, value, comparison))
                return false;

            SetValue(ref field, value, propertyName);

            return true;
        }

        /// <summary>
        /// Special version of "compare and set" for float numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CompareAndSetValueWithPrecision(ref float field, float value, float precision = 0.0001f, [CallerMemberName] string propertyName = null)
        {
            if (Math.Abs(field - value) < precision)
                return false;

            SetValue(ref field, value, propertyName);

            return true;
        }

        /// <summary>
        /// Special version of "compare and set" for double numbers
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CompareAndSetValueWithPrecision(ref double field, double value, double precision = 0.0001, [CallerMemberName] string propertyName = null)
        {
            if (Math.Abs(field - value) < precision)
                return false;

            SetValue(ref field, value, propertyName);

            return true;
        }


        /// <summary>
        /// Regular version of "compare and set" for any values, uses default comparer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CompareAndSetValue<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            return CompareAndSetValue(ref field, value, EqualityComparer<TValue>.Default, propertyName);
        }

        /// <summary>
        /// Regular version of "compare and set" for any values, uses given comparer
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        protected bool CompareAndSetValue<TValue>(ref TValue field, TValue value, IEqualityComparer<TValue> comparer, [CallerMemberName] string propertyName = null)
        {
            if (comparer.Equals(field, value))
                return false;

            SetValue(ref field, value, propertyName);

            return true;
        }
    }
}
