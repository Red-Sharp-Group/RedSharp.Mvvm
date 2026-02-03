using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Validation.Interfaces;

namespace RedSharp.Mvvm.Validation.Abstracts
{
    public abstract class ValidationRuleBase<TInstance, TValue> : IValidationRule
    {
        protected static readonly Task<bool> TrueCompletedTask = Task.FromResult(true);

        protected static readonly Task<bool> FalseCompletedTask = Task.FromResult(false);

        public ValidationRuleBase(string identifier)
        {
            Identifier = identifier;
        }

        public string Identifier { get; }

        public Task<bool> ValidateAsync<TInputValue>(object inputInstance, TInputValue inputValue)
        {
            if (inputInstance is TInstance instance && inputValue is TValue value)
                return InternalInvoke(instance, value);

            return FalseCompletedTask;
        }

        protected abstract Task<bool> InternalInvoke(TInstance instance, TValue value);
    }
}
