using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Utils
{
    public class ValidationInfo : ObservableObject
    {
        private bool? _result;

        public ValidationInfo(string category, IValidationRule rule) 
        {
            ArgumentsGuard.ThrowIfNullOrEmpty(category, nameof(category));
            ArgumentsGuard.ThrowIfNull(rule, nameof(rule));

            Result = true;
            Category = category;
            Rule = rule;
        }

        public bool? Result 
        { 
            get => _result; 
            internal set => CompareAndSetValue(ref _result, value); 
        }

        public string Category { get; }

        public IValidationRule Rule { get; }

        public override string ToString()
        {
            if (!Result.HasValue)
            {
                return $"{Category}: {Rule.Identifier} (Pending)";
            }
            else
            {
                if (Result.Value)
                    return $"{Category}: {Rule.Identifier} (Success)";
                else
                    return $"{Category}: {Rule.Identifier} (Failed)";
            }
        }
    }
}
