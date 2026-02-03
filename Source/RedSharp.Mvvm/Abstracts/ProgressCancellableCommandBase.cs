using System;
using System.Collections.Generic;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// The basic command that can report progress and can be canceled
    /// </summary>
    public abstract class ProgressCancellableCommandBase<TParameter> : AsyncCancellableCommandBase
    {
        private TParameter _progress;

        private Progress<TParameter> _progressHandler;

        /// <inheritdoc cref="ProgressCommandBase{TParameter}.Progress"/>
        public TParameter Progress
        {
            get => _progress;
            private set
            {
                if (IsCanceling)
                    return;

                value = CorrectProgressValue(value);

                if (EqualityComparer<TParameter>.Default.Equals(_progress, value))
                    return;

                RaisePropertyChanging(ProgressCommandBase<TParameter>.ProgressChanging);

                _progress = value;

                RaisePropertyChanged(ProgressCommandBase<TParameter>.ProgressChanged);
            }
        }

        /// <inheritdoc cref="ProgressCommandBase{TParameter}.InitializeProgressHandler"/>
        protected IProgress<TParameter> InitializeProgressHandler()
        {
            if (_progressHandler == null)
                _progressHandler = new Progress<TParameter>(value => Progress = value);

            return _progressHandler;
        }

        /// <inheritdoc cref="ProgressCommandBase{TParameter}.ResetProgressValue"/>
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
