using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Validation.Interfaces
{
    /// <summary>
    /// Simple rule representation
    /// </summary>
    /// <remarks>
    /// The type is basic as possible to be able to wrap external validation frameworks if it is needed
    /// </remarks>
    public interface IValidationRule
    {
        /// <summary>
        /// Rule identifier, used during validation result creation to identify which rule was broken
        /// </summary>
        string Identifier { get; }

        /// <summary>
        /// The method that performs validation of the new <paramref name="value"/>
        /// </summary>
        Task<bool> ValidateAsync<TValue>(object instance, TValue value);
    }
}
