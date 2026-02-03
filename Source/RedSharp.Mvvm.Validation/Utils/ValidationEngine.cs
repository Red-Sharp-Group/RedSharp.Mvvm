using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Utils
{

    public class ValidationEngine
    {
        private object _target;

        public class ValidationProcess
        {
            private ValidationEngine _owner;
            private IValidationRule _rule;

            private object _value;
            private object _lock;
            
            public ValidationProcess(IValidationRule validationRule)
            {
                _rule = validationRule;
            }

            /// <inheritdoc cref="IValidationRule.Identifier"/>
            public string Identifier => _rule.Identifier;

            public bool? Result { get; private set; }

            public void Start<TValue>(TValue value)
            {
                lock (_lock)
                {
                    _value = value;

                    if (!Result.HasValue)
                        return;

                    try
                    {
                        var task = _rule.ValidateAsync(_owner._target, value);

                        if (!task.Wait(AsyncCommandBase.AcceptableDelayTime))
                        {
                            Result = null;

                            Notify();

                            task.ContinueWith(End, value);
                        }
                        else
                        {
                            End(task, _value);
                        }
                    }
                    catch (Exception exception)
                    {
                        End(Task.FromException<bool>(exception), _value);
                    }
                }
            }

            private void End(Task<bool> task, object state)
            {
                lock (_lock)
                {
                    var isLatestCheck = object.Equals(_value, state);

                    if (isLatestCheck)
                    {
                        _value = null;

                        if (task.IsCompleted)
                            Result = task.Result;
                        else
                            Result = false;

                        Notify();
                    }
                    else
                    {
                        Result = false;

                        Start(_value);
                    }
                }
            }

            private void Notify()
            {

            }
        }

        public class RuleWrapper
        {
            private IValidationRule _rule;

            public RuleWrapper(IValidationRule validationRule)
            {
                ValidationRule = validationRule;
            }

            /// <inheritdoc cref="IValidationRule.Identifier"/>
            public string Identifier => _rule.Identifier;

            /// <summary>
            /// Target rule
            /// </summary>
            public IValidationRule ValidationRule { get; }

            /// <summary>
            /// The rule is in process of value validation
            /// </summary>
            public bool IsEvaluating { get; set; }

            /// <summary>
            /// If true the current rule evaluation still works with latest value
            /// </summary>
            public bool IsLatestCheck { get; set; }

            /// <summary>
            /// Obviously the result of validation
            /// </summary>
            /// <remarks>
            /// If <see cref="IsEvaluating"/> is <see cref="true"/> it will be <see cref="false"/>
            /// </remarks>
            public bool IsValueValid { get; set; }

            /// <summary>
            /// Cached validation result
            /// </summary>
            public object ValidationResult { get; set; }
        }

        private Dictionary<string, List<RuleWrapper>> _cache;
        private IValidationContext _context;

        private int _count;

        public ValidationEngine(Dictionary<string, List<IValidationRule>> cache)
        {
            _cache = new Dictionary<string, List<RuleWrapper>>();

            foreach (var item in cache)
            {
                var collection = new List<RuleWrapper>();

                foreach (var rule in item.Value)
                {
                    collection.Add(new RuleWrapper(rule)
                    {
                        IsLatestCheck = true,
                        IsEvaluating = false,
                        IsValueValid = true
                    });
                }

                _cache[item.Key] = collection;
            }
        }

        public int Count
        {
            get => _count;
            private set
            {
                if (_count == value)
                    return;

                _count = value;

                _context.ReportCountChanged();
            }
        }

        public void Initialize(IValidationContext context)
        {
            _context = context;
        }

        public IReadOnlyCollection<object> GetValidationResult(string property)
        {
            if (property == null || !_cache.ContainsKey(property))
                return Array.Empty<object>();

            var result = new List<object>();

            TryFillValidationResult(property, result);

            return result;
        }

        public bool TryFillValidationResult(string property, ICollection<object> result)
        {
            if (property == null)
                return false;

            if (!_cache.TryGetValue(property, out var rules))
                return false;

            foreach (var context in rules)
                if (!context.IsValueValid && context.ValidationResult != null)
                    result.Add(context.ValidationResult);

            return true;
        }

        public void CheckProperty<TValue>(string property, TValue value)
        {
            if (property == null)
                return;

            if (!_cache.TryGetValue(property, out var rules))
                return;

            var count = Count;
            var isChanged = false;

            foreach (var context in rules)
            {
                if (context.IsEvaluating)
                {
                    context.IsLatestCheck = false;
                }
                else
                {
                    context.IsLatestCheck = true;

                    var task = context.ValidationRule.ValidateAsync(_context.Owner, value);

                    if (!task.IsCompleted)
                    {
                        if (CheckResult(false, true, property, context, ref count))
                            isChanged = true;

                        var closureProperty = property;
                        var closureItem = context;

                        task.ContinueWith(taskItem => DefferedCheckResult(taskItem, closureProperty, closureItem));
                    }
                    else
                    {
                        if (CheckResult(task.Result, false, property, context, ref count))
                            isChanged = true;
                    }
                }
            }

            Count = count;

            if (isChanged)
                _context.ReportValidationChanged(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DefferedCheckResult(Task<bool> task, string property, RuleWrapper item)
        {
            if (!item.IsLatestCheck)
            {
                RequestRevalidation(property, item);
            }
            else
            {
                var count = Count;
                var result = CheckResult(task.Result, false, property, item, ref count);

                Count = count;

                if (result)
                    _context.ReportValidationChanged(property);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private bool CheckResult(bool isValid, bool isRunning, string property, RuleWrapper item, ref int count)
        {
            var isChanged = false;

            if (item.IsValueValid != isValid)
            {
                isChanged = true;

                item.IsValueValid = isValid;

                if (item.IsValueValid)
                    count--;
                else
                    count++;
            }

            if (item.IsEvaluating != isRunning)
            {
                isChanged = true;

                item.IsEvaluating = isRunning;
            }

            if (isChanged)
            {
                item.ValidationResult = _context.MakeValidationInfoObject(property,
                    item.ValidationRule.Identifier, item.IsEvaluating, item.IsValueValid, item.ValidationResult);
            }

            return isChanged;
        }

        private void RequestRevalidation(string property, RuleWrapper item)
        {
            var propertyInfo = _context.Owner.GetType().GetProperty(property);
            var value = propertyInfo.GetValue(_context.Owner);

            item.IsLatestCheck = true;

            item.ValidationRule.ValidateAsync(_context.Owner, value).ContinueWith(task => DefferedCheckResult(task, property, item));
        }
    }
}
