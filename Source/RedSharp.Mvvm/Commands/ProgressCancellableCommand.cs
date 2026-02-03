using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Commands
{
    public delegate Task ProgressCancellableCommandAction(IProgress<double> progress, CancellationToken token);

    /// <summary>
    /// Async arguments-less command implementation that can be canceled, 
    /// works with <see cref="Task"/> and <see cref="IProgress{double}"/> with <see cref="CancellationTokenSource"/>
    /// </summary>
    /// <remarks>
    /// The progress value is between 0.0 and 1.0
    /// </remarks>
    public class ProgressCancellableCommand : ProgressCancellableCommandBase<double>, ICommand
    {
        private ProgressCancellableCommandAction _execute;
        private CommandPredicate _canExecute;

        public ProgressCancellableCommand(ProgressCancellableCommandAction execute, CommandPredicate canExecute = null)
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
                var task = _execute.Invoke(InitializeProgressHandler(), InitializeCancelCommand());

                if (!task.Wait(AcceptableDelayTime))
                {
                    IsRunning = true;

                    await task;
                }
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
                ResetCancelCommand();
                ResetProgressValue();

                IsRunning = false;
            }
        }

        bool ICommand.CanExecute(object parameter) => CanExecute();

        void ICommand.Execute(object parameter) => ExecuteAsync();

        protected override double CorrectProgressValue(double input)
        {
            return Math.Max(Math.Min(input, 1.0), 0.0);
        }
    }

    public delegate Task ProgressCancellableCommandAction<TArguments>(TArguments arguments, IProgress<double> progress, CancellationToken token);

    /// <summary>
    /// Async command with arguments implementation that can be canceled, 
    /// works with <see cref="Task"/> and <see cref="IProgress{double}"/> with <see cref="CancellationTokenSource"/>
    /// </summary>
    /// <remarks>
    /// The progress value is between 0.0 and 1.0
    /// </remarks>
    public class ProgressCancellableCommand<TArgument> : ProgressCancellableCommandBase<double>, ICommand
    {
        private ProgressCancellableCommandAction<TArgument> _execute;
        private CommandPredicate<TArgument> _canExecute;

        public ProgressCancellableCommand(ProgressCancellableCommandAction<TArgument> execute, CommandPredicate<TArgument> canExecute = null)
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
                var task = _execute.Invoke(argument, InitializeProgressHandler(), InitializeCancelCommand());

                if (!task.Wait(AcceptableDelayTime))
                {
                    IsRunning = true;

                    await task;
                }
            }
            catch
            {
                /*usually the application ends without try catch*/
            }
            finally
            {
                ResetCancelCommand();
                ResetProgressValue();

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

        protected override double CorrectProgressValue(double input)
        {
            return Math.Max(Math.Min(input, 1.0), 0.0);
        }
    }
}
