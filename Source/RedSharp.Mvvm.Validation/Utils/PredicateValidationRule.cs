using System.Threading.Tasks;
using RedSharp.Mvvm.Validation.Abstracts;

namespace RedSharp.Mvvm.Validation.Utils
{
    public delegate bool ValidationPredicate<in TContext, in TInput>(TContext context, TInput @object);

    public class PredicateValidationRule<TInstance, TValue> : ValidationRuleBase<TInstance, TValue>
    {
        private ValidationPredicate<TInstance, TValue> _predicate;

        public PredicateValidationRule(ValidationPredicate<TInstance, TValue> predicate, string identifier) : base(identifier)
        {
            _predicate = predicate;
        }

        protected override Task<bool> InternalInvoke(TInstance instance, TValue value)
        {
            try
            {
                if (_predicate.Invoke(instance, value))
                    return TrueCompletedTask;
                else
                    return FalseCompletedTask;
            }
            catch
            {
                /*do not care right now*/
            }

            return FalseCompletedTask;
        }
    }
}
