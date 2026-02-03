using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// The prototype of long executing command with usage of the <see cref="IProgress{TParameter}"/> object for the progress reporting
    /// </summary>
    public abstract class ProgressCommandBase<TParameter> : AsyncCommandBase
    {
        /// <summary>
        /// Cached arguments "changING" for the <see cref="Progress"/> property
        /// </summary>
        public static readonly PropertyChangingEventArgs ProgressChanging = new PropertyChangingEventArgs(nameof(Progress));

        /// <summary>
        /// Cached arguments "changED" for the <see cref="Progress"/> property
        /// </summary>
        public static readonly PropertyChangedEventArgs ProgressChanged = new PropertyChangedEventArgs(nameof(Progress));

        private TParameter _progress;

        private Progress<TParameter> _progressHandler;

        /// <summary>
        /// Current progress of command execution
        /// </summary>
        public TParameter Progress
        {
            get => _progress;
            private set
            {
                value = CorrectProgressValue(value);

                if (EqualityComparer<TParameter>.Default.Equals(_progress, value))
                    return;

                RaisePropertyChanging(ProgressChanging);

                _progress = value;

                RaisePropertyChanged(ProgressChanged);
            }
        }

        /// <summary>
        /// Lazy <see cref="IProgress{TParameter}"/> object initializer
        /// </summary>
        protected IProgress<TParameter> InitializeProgressHandler()
        {
            if (_progressHandler == null)
                _progressHandler = new Progress<TParameter>(value => Progress = value);

            return _progressHandler;
        }

        /// <summary>
        /// Sets <see cref="Progress"/> to default
        /// </summary>
        protected void ResetProgressValue()
        {
            Progress = GetDefaultProgressValue();
        }

        protected virtual TParameter GetDefaultProgressValue()
        {
            return default;
        }

        protected virtual TParameter CorrectProgressValue(TParameter input)
        {
            return input;
        }
    }
}
