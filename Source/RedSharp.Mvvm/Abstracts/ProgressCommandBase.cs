using System;
using System.ComponentModel;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// The prototype of long executing command with usage of the <see cref="IProgress{double}"/> object for the progress reporting
    /// </summary>
    /// <remarks>
    /// The progress value is between 0.0 and 1.0
    /// </remarks>
    public abstract class ProgressCommandBase : AsyncCommandBase
    {
        /// <summary>
        /// Cached arguments "changING" for the <see cref="Progress"/> property
        /// </summary>
        public static readonly PropertyChangingEventArgs ProgressChanging = new PropertyChangingEventArgs(nameof(Progress));

        /// <summary>
        /// Cached arguments "changED" for the <see cref="Progress"/> property
        /// </summary>
        public static readonly PropertyChangedEventArgs ProgressChanged = new PropertyChangedEventArgs(nameof(Progress));

        private double _progress;

        private Progress<double> _progressHandler;

        /// <summary>
        /// Current progress of command execution
        /// </summary>
        /// <remarks>
        /// The value is between 0.0 and 1.0
        /// </remarks>
        public double Progress
        {
            get => _progress;
            private set
            {
                if (value < 0.0)
                    value = 0.0;

                if (value > 1.0)
                    value = 1.0;

                if (_progress == value)
                    return;

                RaisePropertyChanging(ProgressChanging);

                _progress = value;

                RaisePropertyChanged(ProgressChanged);
            }
        }

        /// <summary>
        /// Lazy <see cref="IProgress{double}"/> object initializer
        /// </summary>
        protected IProgress<double> InitializeProgressHandler()
        {
            if (_progressHandler == null)
                _progressHandler = new Progress<double>(value => Progress = value);

            return _progressHandler;
        }

        /// <summary>
        /// Sets <see cref="Progress"/> to zero
        /// </summary>
        protected void TerminateProgressHandler()
        {
            Progress = 0.0;
        }
    }
}
