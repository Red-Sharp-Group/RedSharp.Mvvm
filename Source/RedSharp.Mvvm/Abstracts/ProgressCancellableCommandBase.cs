using System;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// The basic command that can report progress and can be canceled
    /// </summary>
    public abstract class ProgressCancellableCommandBase : AsyncCancellableCommandBase
    {
        private double _progress;

        private Progress<double> _progressHandler;

        /// <inheritdoc cref="ProgressCommandBase.Progress"/>
        public double Progress
        {
            get => _progress;
            private set
            {
                if (IsCanceling)
                    return;

                if (value < 0.0)
                    value = 0.0;

                if (value > 1.0)
                    value = 1.0;

                if (_progress == value)
                    return;

                RaisePropertyChanging(ProgressCommandBase.ProgressChanging);

                _progress = value;

                RaisePropertyChanged(ProgressCommandBase.ProgressChanged);
            }
        }

        /// <inheritdoc cref="ProgressCommandBase.InitializeProgressHandler"/>
        protected IProgress<double> InitializeProgressHandler()
        {
            if (_progressHandler == null)
                _progressHandler = new Progress<double>(value => Progress = value);

            return _progressHandler;
        }

        /// <inheritdoc cref="ProgressCommandBase.TerminateProgressHandler"/>
        protected void TerminateProgressHandler()
        {
            Progress = 0.0;
        }
    }
}
