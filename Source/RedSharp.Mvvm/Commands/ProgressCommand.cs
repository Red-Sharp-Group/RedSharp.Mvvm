using System;
using RedSharp.General.Helpers;
using System.Threading.Tasks;
using System.Threading;
using System.Windows.Input;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Commands
{
    public delegate Task ProgressCommandAction(IProgress<double> progress);

    /// <summary>
    /// Async arguments-less command implementation for the long task execution, 
    /// works with <see cref="Task"/> and <see cref="IProgress{double}"/>
    /// </summary>
    public class ProgressCommand : ProgressCommandBase, ICommand
    {
        private ProgressCommandAction _execute;
        private CommandPredicate _canExecute;

        public ProgressCommand(ProgressCommandAction execute, CommandPredicate canExecute = null)
        {
            ArgumentsGuard.ThrowIfNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="AsyncCommand.CanExecute"/>
        public bool CanExecute()
        {
            if (_canExecute == null)
                return !IsRunning;
            else
                return !IsRunning && _canExecute.Invoke();
        }

        /// <inheritdoc cref="AsyncCommand.Execute"/>
        public void Execute() => ExecuteAsync().Wait();

        /// <inheritdoc cref="AsyncCommand.ExecuteAsync"/>
        public async Task ExecuteAsync()
        {
            if (!CanExecute())
                return;

            try
            {
                IsRunning = true;

                await _execute.Invoke(InitializeProgressHandler());
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
                TerminateProgressHandler();

                IsRunning = false;
            }
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => ExecuteAsync();
    }

    public delegate Task ProgressCommandAction<TArguments>(TArguments arguments, IProgress<double> progress);

    /// <summary>
    /// Async command with arguments implementation for the long task execution, 
    /// works with <see cref="Task"/> and <see cref="IProgress{double}"/>
    /// </summary>
    public class ProgressCommand<TArgument> : ProgressCommandBase, ICommand
    {
        private ProgressCommandAction<TArgument> _execute;
        private CommandPredicate<TArgument> _canExecute;

        public ProgressCommand(ProgressCommandAction<TArgument> execute, CommandPredicate<TArgument> canExecute = null)
        {
            ArgumentsGuard.ThrowIfNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="AsyncCommand{TArgument}.CanExecute(TArgument)"/>
        public bool CanExecute(TArgument argument)
        {
            if (_canExecute == null)
                return !IsRunning;
            else
                return !IsRunning && _canExecute.Invoke(argument);
        }

        /// <inheritdoc cref="AsyncCommand{TArgument}.Execute(TArgument)"/>
        public void Execute(TArgument argument) => ExecuteAsync(argument).Wait();

        /// <inheritdoc cref="AsyncCommand{TArgument}.ExecuteAsync(TArgument)"/>
        public async Task ExecuteAsync(TArgument argument)
        {
            if (!CanExecute(argument))
                return;

            try
            {
                IsRunning = true;

                await _execute.Invoke(argument, InitializeProgressHandler());
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
                TerminateProgressHandler();

                IsRunning = false;
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
                ExecuteAsync((TArgument)parameter);
        }
    }
}
