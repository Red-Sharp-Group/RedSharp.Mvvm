using System.ComponentModel;
using System;
using System.Threading;
using System.Windows.Input;

namespace RedSharp.Mvvm.Abstracts
{
    public class AsyncCancellableCommandBase : AsyncCommandBase
    {
        /// <summary>
        /// The simplest implementation of cancel command
        /// </summary>
        private class InternalCancelCommand : CommandBase, ICommand
        {
            public CancellationTokenSource CancellationTokenSource { get; set; }

            public bool CanExecute(object parameter)
            {
                return CancellationTokenSource != null;
            }

            public void Execute(object parameter)
            {
                if (CancellationTokenSource != null)
                    CancellationTokenSource.Cancel();
            }
        }

        /// <summary>
        /// Cached arguments "changING" for the <see cref="IsCanceling"/> property
        /// </summary>
        public static readonly PropertyChangingEventArgs IsCancelingChanging = new PropertyChangingEventArgs(nameof(IsCanceling));

        /// <summary>
        /// Cached arguments "changED" for the <see cref="IsCanceling"/> property
        /// </summary>
        public static readonly PropertyChangedEventArgs IsCancelingChanged = new PropertyChangedEventArgs(nameof(IsCanceling));

        private bool _isCanceling;

        private InternalCancelCommand _cancelCommand;

        public AsyncCancellableCommandBase()
        {
            _cancelCommand = new InternalCancelCommand();
        }

        /// <summary>
        /// True if the command is in the progress and was canceled
        /// </summary>
        public bool IsCanceling
        {
            get => _isCanceling;
            private set
            {
                if (_isCanceling == value)
                    return;

                RaisePropertyChanging(IsCancelingChanging);

                _isCanceling = value;

                RaisePropertyChanged(IsCancelingChanged);
            }
        }

        /// <summary>
        /// Sub command to cancel the main command execution
        /// </summary>
        public ICommand CancelCommand => _cancelCommand;

        /// <summary>
        /// Sets a new instance of the <see cref="CancellationTokenSource"/> into the cancel command
        /// </summary>
        protected CancellationToken InitializeCancelCommand()
        {
            var tokenSource = new CancellationTokenSource();
            var token = tokenSource.Token;

            token.Register(() => IsCanceling = true, true);

            _cancelCommand.CancellationTokenSource = tokenSource;

            _cancelCommand.RaiseCanExecute();

            return token;
        }

        /// <summary>
        /// Resets the cancel state
        /// </summary>
        protected void ResetCancelCommand()
        {
            _cancelCommand.CancellationTokenSource.Dispose();
            _cancelCommand.CancellationTokenSource = null;

            _cancelCommand.RaiseCanExecute();

            IsCanceling = false;
        }
    }
}
