using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.General.Helpers;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Interfaces;
using RedSharp.Mvvm.Validation.Utils;

namespace RedSharp.Mvvm.Validation.Components
{
    public class NotifyDataErrorInfoComponent : ObservableObject, INotifyDataErrorInfo, IObservableComponent
    {
        private Dictionary<string, PropertyValidation> _validations;
        private object _target;
        private bool _hasError;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public NotifyDataErrorInfoComponent(object target)
        {
            _validations = new Dictionary<string, PropertyValidation>(StringComparer.InvariantCultureIgnoreCase);
            _target = target;
            _hasError = false;
        }

        public bool HasErrors
        {
            get => _hasError;
            private set => CompareAndSetValue(ref _hasError, value);
        }

        public IEnumerable GetErrors(string propertyName)
        {
            if (!_validations.TryGetValue(propertyName, out PropertyValidation propertyValidation))
                return Array.Empty<object>();

            var list = propertyValidation.Info;
            var count = list.Count - propertyValidation.NumberOfSucceeded;
            var result = new ValidationInfo[count];

            for (int i = 0, j = 0; i < list.Count; i++)
            {
                var item = list[i];

                if (item.Result == true)
                    continue;

                result[j] = item;

                j++;
            }

            return result;
        }

        public void OwnerPropertyChanged<TValue>(TValue value, string propertyName)
        {
            if (!_validations.TryGetValue(propertyName, out PropertyValidation propertyValidation))
                return;

            propertyValidation.Validate(value);
        }

        private void OnErrorsChanged(object sender, DataErrorsChangedEventArgs arguments)
        {
            HasErrors = _validations.Values.Any(item => item.NumberOfSucceeded != item.Info.Count);

            try
            {
                ErrorsChanged?.Invoke(_target, arguments);
            }
            catch 
            { }
        }
    }
}
