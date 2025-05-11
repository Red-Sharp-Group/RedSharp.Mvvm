using System;
using System.Windows.Input;
using RedSharp.General.Helpers;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// Represents default "command" object, that extends <see cref="ObservableObject"/>
    /// </summary>
    /// <remarks>
    /// Doesn't directly implement <see cref="ICommand"/> - it is too complicated to separate the functionality later
    /// </remarks>
    public abstract class CommandBase : ObservableObject
    {
        /// <inheritdoc cref="ICommand.CanExecuteChanged"/>
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Invokes the <see cref="CanExecuteChanged"/> event in try catch statement
        /// </summary>
        public void RaiseCanExecute() => CanExecuteChanged.InvokeSafe(this);
    }
}
