using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Commands
{
    public delegate void CommandAction();

    public delegate bool CommandPredicate();

    /// <summary>
    /// Simple arguments-less command implementation, based around given delegates
    /// </summary>
    public class Command : CommandBase, ICommand
    {
        private CommandAction _execute;
        private CommandPredicate _canExecute;

        public Command(CommandAction execute, CommandPredicate canExecute = null)
        {
            ArgumentsGuard.ThrowIfNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="ICommand.CanExecute"/>
        public bool CanExecute()
        {
            if (_canExecute == null)
                return true;
            else
                return _canExecute.Invoke();
        }

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// </remarks>
        public void Execute()
        {
            if (!CanExecute())
                return;

            try
            {
                _execute?.Invoke();
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => Execute();
    }


    public delegate void CommandAction<TArguments>(TArguments arguments);

    public delegate bool CommandPredicate<TArguments>(TArguments arguments);

    /// <summary>
    /// Simple command implementation with arguments, based around given delegates
    /// </summary>
    public class Command<TArgument> : CommandBase, ICommand
    {
        private CommandAction<TArgument> _execute;
        private CommandPredicate<TArgument> _canExecute;

        public Command(CommandAction<TArgument> execute, CommandPredicate<TArgument> canExecute = null)
        {
            ArgumentsGuard.ThrowIfNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// <inheritdoc cref="ICommand.CanExecute"/>
        /// </summary>
        /// <remarks>
        /// The explicit implementation of the <inheritdoc cref="ICommand.CanExecute"/> returns false 
        /// if the given object is not match the <typeparamref name="TArgument"/> type
        /// </remarks>
        public bool CanExecute(TArgument argument)
        {
            if (_canExecute == null)
                return true;
            else
                return _canExecute.Invoke(argument);
        }

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// <br/>The explicit implementation of the <inheritdoc cref="ICommand.Execute"/> will not be executed 
        /// if the given object is not match the <typeparamref name="TArgument"/> type
        /// </remarks>
        public void Execute(TArgument argument)
        {
            if (!CanExecute(argument))
                return;

            try
            {
                _execute?.Invoke(argument);
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
        }

        bool ICommand.CanExecute(object parameter)
        {
            if (parameter is TArgument)
                return CanExecute((TArgument)parameter);
            else
                return false;
        }

        void ICommand.Execute(object parameter)
        {
            if (parameter is TArgument)
                Execute((TArgument)parameter);
        }
    }
}
