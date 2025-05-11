using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Commands
{
    public delegate Task AsyncCommandAction();

    /// <summary>
    /// Async arguments-less command implementation, works with <see cref="Task"/>
    /// </summary>
    public class AsyncCommand : AsyncCommandBase, ICommand
    {
        private AsyncCommandAction _execute;
        private CommandPredicate _canExecute;

        public AsyncCommand(AsyncCommandAction execute, CommandPredicate canExecute = null)
        {
            ArgumentsGuard.ThrowIfNull(execute, nameof(execute));

            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc cref="ICommand.CanExecute"/>
        public bool CanExecute()
        {
            if (_canExecute == null)
                return !IsRunning;
            else
                return !IsRunning && _canExecute.Invoke();
        }

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// <br/> During execution <see cref="IsRunning"/> property will be set true.
        /// <br/> This method will wait until execution is done, the explicit implementation of the <see cref="ICommand.Execute"/> will not.
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// </remarks>
        public void Execute() => ExecuteAsync().Wait();

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// <br/> During execution <see cref="IsRunning"/> property will be set true.
        /// <br/> This method is awaitable, the explicit implementation of the <see cref="ICommand.Execute"/> is not.
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// </remarks>
        public async Task ExecuteAsync()
        {
            if (!CanExecute())
                return;

            try
            {
                IsRunning = true;

                await _execute.Invoke();
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
                IsRunning = false;
            }
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => ExecuteAsync();
    }

    public delegate Task AsyncCommandAction<TArguments>(TArguments arguments);

    /// <summary>
    /// Async command implementation with arguments, works with <see cref="Task"/>
    /// </summary>
    public class AsyncCommand<TArgument> : AsyncCommandBase, ICommand
    {
        private AsyncCommandAction<TArgument> _execute;
        private CommandPredicate<TArgument> _canExecute;

        public AsyncCommand(AsyncCommandAction<TArgument> execute, CommandPredicate<TArgument> canExecute = null)
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
                return !IsRunning;
            else
                return !IsRunning && _canExecute.Invoke(argument);
        }

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// <br/> During execution <see cref="IsRunning"/> property will be set true.
        /// <br/> This method will wait until execution is done, the explicit implementation of the <see cref="ICommand.Execute"/> will not.
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// <br/>The explicit implementation of the <inheritdoc cref="ICommand.Execute"/> will not be executed 
        /// if the given object is not match the <typeparamref name="TArgument"/> type
        /// </remarks>
        public void Execute(TArgument argument) => ExecuteAsync(argument).Wait();

        /// <summary>
        /// <inheritdoc cref="ICommand.Execute"/>
        /// <br/> During execution <see cref="IsRunning"/> property will be set true.
        /// <br/> This method is awaitable, the explicit implementation of the <see cref="ICommand.Execute"/> is not.
        /// </summary>
        /// <remarks>
        /// Performs execution in try catch statement.
        /// <br/>The execution will not be performed in case <see cref="CanExecute"/> returns false
        /// <br/>The explicit implementation of the <inheritdoc cref="ICommand.Execute"/> will not be executed 
        /// if the given object is not match the <typeparamref name="TArgument"/> type
        /// </remarks>
        public async Task ExecuteAsync(TArgument argument)
        {
            if (!CanExecute(argument))
                return;

            try
            {
                IsRunning = true;

                await _execute.Invoke(argument);
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
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
