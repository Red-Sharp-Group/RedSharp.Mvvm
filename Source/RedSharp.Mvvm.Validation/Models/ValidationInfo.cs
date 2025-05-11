using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Models
{
    public class ValidationInfo : ObservableObject
    {
        private string _identifier;
        private bool _isRunning;
        private string _category;

        public ValidationInfo(string identifier, string category)
        {
            _identifier = identifier;
            _category = category;
        }

        public string Identifier { get; }

        public string Category { get; }

        public bool IsRunning
        {
            get => _isRunning;
            set => CompareAndSetValue(ref _isRunning, value);
        }

        public override string ToString()
        {
            if (IsRunning)
                return $"{Category}(Pending): {Identifier}";
            else
                return $"{Category}: {Identifier}";
        }
    }
}
