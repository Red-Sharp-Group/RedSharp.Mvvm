using System.Threading.Tasks;
using RedSharp.Mvvm.Validation.Abstracts;

namespace RedSharp.Mvvm.Validation.Utils
{
    public delegate Task<bool> AsyncValidationPredicate<in TContext, in TInput>(TContext context, TInput @object);

    public class AsyncPredicateValidationRule<TInstance, TValue> : ValidationRuleBase<TInstance, TValue>
    {
        private AsyncValidationPredicate<TInstance, TValue> _predicate;

        public AsyncPredicateValidationRule(AsyncValidationPredicate<TInstance, TValue> predicate, string identifier) : base(identifier)
        {
            _predicate = predicate;
        }

        protected override Task<bool> InternalInvoke(TInstance instance, TValue value)
        {
            return _predicate.Invoke(instance, value);
        }
    }
}
