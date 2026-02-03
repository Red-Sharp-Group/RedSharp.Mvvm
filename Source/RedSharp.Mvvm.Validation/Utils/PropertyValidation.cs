using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Validation.Utils
{
    public class PropertyValidation
    {
        public const int AcceptableDelayTime = 5; //milliseconds

        private PropertyInfo _propertyInfo;
        private object _target;
        private object _lock;

        private int _executionMark;
        private int _numberOfSucceeded;

        private ValidationInfo[] _rules;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public PropertyValidation(object target, PropertyInfo property, IReadOnlyList<ValidationInfo> list)
        {
            _target = target;
            _propertyInfo = property;

            _lock = new object();

            _executionMark = 0;
            _numberOfSucceeded = 0;

            _rules = new ValidationInfo[list.Count];

            for (int i = 0; i < list.Count; i++)
                _rules[i] = list[i];
        }

        public int NumberOfSucceeded => _numberOfSucceeded;

        public IReadOnlyList<ValidationInfo> Info => _rules;

        public void Validate<TValue>(TValue value)
        {
            lock (_lock)
            {
                _executionMark++;
                _numberOfSucceeded = 0;

                var anyStateChanged = false;

                for (int i = 0; i < _rules.Length; i++)
                {
                    if (!_rules[i].Result.HasValue)
                        continue;

                    anyStateChanged = anyStateChanged || StartRuleValidation(value, i, _executionMark);
                }

                if (anyStateChanged)
                    NotifyStateChanged();
            }
        }

        private bool StartRuleValidation<TValue>(TValue value, int index, int executionMark)
        {
            bool? validationResult = null;

            try
            {
                var task = _rules[index].Rule.ValidateAsync(_target, value);

                if (!task.Wait(AsyncCommandBase.AcceptableDelayTime))
                    task.ContinueWith(temp => EndRuleValidation(temp, index, executionMark));
                else
                    validationResult = task.Result;
            }
            catch
            {
                validationResult = false;
            }

            if (validationResult == true)
                _numberOfSucceeded++;

            var previousResult = _rules[index].Result;

            _rules[index].Result = validationResult;

            return validationResult != previousResult;
        }

        private void EndRuleValidation(Task<bool> task, int index, int executionMark)
        {
            lock (_lock)
            {
                if (_executionMark == executionMark)
                {
                    var isSucceded = false;

                    if (!task.IsCanceled && !task.IsFaulted)
                        isSucceded = task.Result;

                    if (isSucceded)
                        _numberOfSucceeded++;

                    _rules[index].Result = isSucceded;

                    NotifyStateChanged();
                }
                else
                {
                    var value = _propertyInfo.GetValue(_target);

                    if (StartRuleValidation(value, index, _executionMark))
                        NotifyStateChanged();
                }
            }
        }

        private void NotifyStateChanged()
        {
            if (ErrorsChanged == null)
                return;

            ErrorsChanged.Invoke(this, new DataErrorsChangedEventArgs(_propertyInfo.Name));
        }
    }
}
