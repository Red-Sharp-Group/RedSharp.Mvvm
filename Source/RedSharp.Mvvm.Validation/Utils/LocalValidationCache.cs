using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Utils
{
    public class LocalValidationCache
    {
        public class ValidationCache
        {
            public ValidationCache(IValidationRule validationRule)
            {
                ValidationRule = validationRule;
            }

            public IValidationRule ValidationRule { get; }

            public bool IsRunning { get; set; }

            public bool IsRelevant { get; set; }

            public bool IsValid { get; set; }

            public object validationInfo { get; set; }
        }

        private Dictionary<string, List<ValidationCache>> _cache;
        private IValidationContext _context;

        private int _count;

        public LocalValidationCache(Dictionary<string, List<IValidationRule>> cache)
        {
            _cache = new Dictionary<string, List<ValidationCache>>();

            foreach (var item in cache)
            {
                var collection = new List<ValidationCache>();

                foreach (var rule in item.Value)
                {
                    collection.Add(new ValidationCache(rule)
                    {
                        IsRelevant = true,
                        IsRunning = false,
                        IsValid = true
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

        public bool TryFillValidationResult(string property, ICollection<object> result)
        {
            if (property == null)
                return false;

            if (!_cache.TryGetValue(property, out var validationCache))
                return false;

            foreach (var item in validationCache)
                if (!item.IsValid && item.validationInfo != null)
                    result.Add(item.validationInfo);

            return true;
        }

        public void Check<TValue>(string property, TValue value)
        {
            if (property == null)
                return;

            if (!_cache.TryGetValue(property, out var validationCache))
                return;

            var count = Count;
            var isChanged = false;

            foreach (var item in validationCache)
            {
                if (item.IsRunning)
                {
                    item.IsRelevant = false;
                }
                else
                {
                    item.IsRelevant = true;

                    var task = item.ValidationRule.ValidateAsync(_context.Owner, value);

                    if (!task.IsCompleted)
                    {
                        if (CheckResult(false, true, property, item, ref count))
                            isChanged = true;

                        var closureProperty = property;
                        var closureItem = item;

                        task.ContinueWith(taskItem => DefferedCheckResult(taskItem, closureProperty, closureItem));
                    }
                    else
                    {
                        if (CheckResult(task.Result, false, property, item, ref count))
                            isChanged = true;
                    }
                }
            }

            Count = count;

            if (isChanged)
                _context.ReportValidationChanged(property);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DefferedCheckResult(Task<bool> task, string property, ValidationCache item)
        {
            if (!item.IsRelevant)
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
        private bool CheckResult(bool isValid, bool isRunning, string property, ValidationCache item, ref int count)
        {
            var isChanged = false;

            if (item.IsValid != isValid)
            {
                isChanged = true;

                item.IsValid = isValid;

                if (item.IsValid)
                    count--;
                else
                    count++;
            }

            if (item.IsRunning != isRunning)
            {
                isChanged = true;

                item.IsRunning = isRunning;
            }

            if (isChanged)
            {
                item.validationInfo = _context.MakeValidationInfoObject(property,
                    item.ValidationRule.Identifier, item.IsRunning, item.IsValid, item.validationInfo);
            }

            return isChanged;
        }

        private void RequestRevalidation(string property, ValidationCache item)
        {
            var propertyInfo = _context.Owner.GetType().GetProperty(property);
            var value = propertyInfo.GetValue(_context.Owner);

            item.IsRelevant = true;

            item.ValidationRule.ValidateAsync(_context.Owner, value).ContinueWith(task => DefferedCheckResult(task, property, item));
        }
    }
}
