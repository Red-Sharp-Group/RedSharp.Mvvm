using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Models;
using RedSharp.Mvvm.Validation.Interfaces;
using RedSharp.Mvvm.Validation.Utils;

namespace RedSharp.Mvvm.Validation.Abstracts
{
    public abstract class ValidatableObject<TValidationInfo> : ObservableObject, INotifyDataErrorInfo
    {
        private class ValidationContext : IValidationContext
        {
            private ValidatableObject<TValidationInfo> _owner;
            private string _category;

            public ValidationContext(ValidatableObject<TValidationInfo> owner, string category)
            {
                _owner = owner;
                _category = category;
            }

            public ObservableObject Owner => _owner;

            public object MakeValidationInfoObject(string property, string rule, bool isRunning, bool IsValid, object stored)
            {
                var castedObject = default(TValidationInfo);

                if (stored != null)
                    castedObject = (TValidationInfo)stored;

                return _owner.WrapValidationResult(property, rule, _category, isRunning, IsValid, castedObject);
            }

            public void ReportCountChanged()
            {
                _owner.RaisePropertyChanged(nameof(ValidatableObject<TValidationInfo>.HasErrors));
            }

            public void ReportValidationChanged(string property)
            {
                _owner.RaiseErrorsChanged(property);
            }
        }

        public const string ItemLevelPropertyName = "::this";

        protected const string ErrorValidationCategory = "Error";
        protected const string WarningValidationCategory = "Warning";
        protected const string InformationValidationCategory = "Information";

        private static Dictionary<string, GlobalValidationCache> _globalValidationCache;
        private static List<object> _sharedValidationInfoCache;

        private Dictionary<string, LocalValidationCache> _localValidationCache;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public ValidatableObject()
        {
            if (_globalValidationCache == null)
                return;

            var currentType = GetType();

            foreach(var item in _globalValidationCache)
            {
                var category = item.Key;
                var globalCache = item.Value;
                var localCache = globalCache.CreateLocalCache(currentType);

                if (localCache != null)
                {
                    localCache.Initialize(new ValidationContext(this, category));

                    if (_localValidationCache == null)
                        _localValidationCache = new Dictionary<string, LocalValidationCache>(StringComparer.InvariantCultureIgnoreCase);

                    _localValidationCache.Add(category, localCache);
                }
            }
        }

        public virtual bool HasErrors
        {
            get
            {
                if (_localValidationCache == null)
                    return false;

                return _localValidationCache.Values.Any(cache => cache.Count > 0);
            }
        }

        private static void RegisterValidationRule<TContext, TValue>(string category, ValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties)
        {
            ArgumentsGuard.ThrowIfNull(predicate, nameof(predicate));
            ArgumentsGuard.ThrowIfNullOrEmpty(identifier, nameof(identifier));
            ArgumentsGuard.ThrowIfNullOrEmpty(properties, nameof(properties));

            var validator = new PredicateValidationRule<TContext, TValue>(predicate, identifier);

            RegisterValidationRule<TContext>(category, validator, properties);
        }

        private static void RegisterAsyncValidationRule<TContext, TValue>(string category, AsyncValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties)
        {
            ArgumentsGuard.ThrowIfNull(predicate, nameof(predicate));
            ArgumentsGuard.ThrowIfNullOrEmpty(identifier, nameof(identifier));
            ArgumentsGuard.ThrowIfNullOrEmpty(properties, nameof(properties));

            var validator = new AsyncPredicateValidationRule<TContext, TValue>(predicate, identifier);

            RegisterValidationRule<TContext>(category, validator, properties);
        }

        private static void RegisterValidationRule<TContext>(string category, IValidationRule rule, params string[] properties)
        {
            ArgumentsGuard.ThrowIfNull(rule, nameof(rule));

            if (_globalValidationCache == null)
                _globalValidationCache = new Dictionary<string, GlobalValidationCache>(StringComparer.InvariantCultureIgnoreCase);

            if (!_globalValidationCache.TryGetValue(category, out var cache))
            {
                cache = new GlobalValidationCache();

                _globalValidationCache.Add(category, cache);
            }

            cache.RegisterValidationRule(typeof(TContext), rule, properties);
        }

        protected static void RegisterErrorValidation<TContext, TValue>(ValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties) 
            where TContext : ObservableObject => RegisterValidationRule(ErrorValidationCategory, predicate, identifier, properties);
        
        protected static void RegisterErrorValidation<TContext, TValue>(AsyncValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties)
            where TContext : ObservableObject => RegisterAsyncValidationRule(ErrorValidationCategory, predicate, identifier, properties);

        protected static void RegisterErrorValidation<TContext, TValue>(IValidationRule rule, params string[] properties)
            where TContext : ObservableObject => RegisterValidationRule<TContext>(ErrorValidationCategory, rule, properties);


        protected static void RegisterWarningValidation<TContext, TValue>(ValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties) 
            where TContext : ObservableObject => RegisterValidationRule(WarningValidationCategory, predicate, identifier, properties);
        
        protected static void RegisterWarningValidation<TContext, TValue>(AsyncValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties)
            where TContext : ObservableObject => RegisterAsyncValidationRule(WarningValidationCategory, predicate, identifier, properties);

        protected static void RegisterWarningValidation<TContext, TValue>(IValidationRule rule, params string[] properties)
            where TContext : ObservableObject => RegisterValidationRule<TContext>(WarningValidationCategory, rule, properties);


        protected static void RegisterInformationValidation<TContext, TValue>(ValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties) 
            where TContext : ObservableObject => RegisterValidationRule(InformationValidationCategory, predicate, identifier, properties);
        
        protected static void RegisterInformationValidation<TContext, TValue>(AsyncValidationPredicate<TContext, TValue> predicate, string identifier, params string[] properties)
            where TContext : ObservableObject => RegisterAsyncValidationRule(InformationValidationCategory, predicate, identifier, properties);

        protected static void RegisterInformationValidation<TContext, TValue>(IValidationRule rule, params string[] properties)
            where TContext : ObservableObject => RegisterValidationRule<TContext>(InformationValidationCategory, rule, properties);


        public IEnumerable GetErrors(string propertyName)
        {
            if (_localValidationCache == null)
                return Enumerable.Empty<object>();

            if (string.IsNullOrEmpty(propertyName))
                propertyName = ItemLevelPropertyName;

            if (_sharedValidationInfoCache != null)
                _sharedValidationInfoCache.Clear();
            else
                _sharedValidationInfoCache = new List<object>();

            foreach(var item in _localValidationCache.Values)
                item.TryFillValidationResult(propertyName, _sharedValidationInfoCache);

            return _sharedValidationInfoCache;
        }

        protected void CheckValue<TValue>(TValue value, [CallerMemberName] string propertyName = null)
        {
            if (_localValidationCache == null)
                return;

            foreach (var cache in _localValidationCache.Values)
                cache.Check(propertyName, value);
        }

        protected override void SetValue<TValue>(ref TValue field, TValue value, [CallerMemberName] string propertyName = null)
        {
            base.SetValue(ref field, value, propertyName);

            CheckValue(value, propertyName);
        }

        protected abstract TValidationInfo WrapValidationResult(string property, string identifier, string category, bool isRunning, bool isValid, TValidationInfo stored);

        protected void RaiseErrorsChanged([CallerMemberName] string property = null)
        {
            if (ErrorsChanged != null && property != null)
            {
                try
                {
                    var arguments = new DataErrorsChangedEventArgs(property);

                    ErrorsChanged.Invoke(this, arguments);
                }
                catch
                { 
                    /*do not care, just for safe*/
                }
            }
        }
    }

    public abstract class ValidatableObject : ValidatableObject<ValidationInfo>
    {
        protected override ValidationInfo WrapValidationResult(string property, string identifier, string category, bool isRunning, bool isValid, ValidationInfo previous)
        {
            if (!isRunning && isValid)
                return null;

            if (previous == null)
                previous = new ValidationInfo(identifier, category);

            previous.IsRunning = isRunning;

            return previous;
        }
    }
}
