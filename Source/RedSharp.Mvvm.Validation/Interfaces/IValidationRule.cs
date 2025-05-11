using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Validation.Interfaces
{
    public interface IValidationRule
    {
        string Identifier { get; }

        Task<bool> ValidateAsync<TValue>(ObservableObject instance, TValue value);
    }
}
