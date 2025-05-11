using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RedSharp.Mvvm.Abstracts
{
    /// <summary>
    /// Basic command object for a <see cref="Task"/> execution
    /// </summary>
    public abstract class AsyncCommandBase : CommandBase
    {
        /// <summary>
        /// Cached arguments "changING" for the <see cref="IsRunning"/> property
        /// </summary>
        public static readonly PropertyChangingEventArgs IsRunningChanging = new PropertyChangingEventArgs(nameof(IsRunning));

        /// <summary>
        /// Cached arguments "changED" for the <see cref="IsRunning"/> property
        /// </summary>
        public static readonly PropertyChangedEventArgs IsRunningChanged = new PropertyChangedEventArgs(nameof(IsRunning));

        private bool _isRunning;

        /// <summary>
        /// True if the command is in the progress
        /// </summary>
        /// <remarks>
        /// The command is not executable if it is in running mode
        /// </remarks>
        public bool IsRunning
        {
            get => _isRunning;
            protected set
            {
                if (_isRunning == value)
                    return;

                RaisePropertyChanging(IsRunningChanging);

                _isRunning = value;

                RaisePropertyChanged(IsRunningChanged);
                RaiseCanExecute();
            }
        }
    }
}
