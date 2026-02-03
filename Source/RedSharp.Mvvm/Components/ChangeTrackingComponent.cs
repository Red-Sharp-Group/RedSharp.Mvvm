using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Interfaces;

namespace RedSharp.Mvvm.Components
{
    public class ChangeTrackingComponent : ObservableObject, IChangeTracking, IObservableComponent
    {
        private ISet<string> _targetProperties;
        private bool _isChanged;

        public ChangeTrackingComponent(ISet<string> targetProperties)
        {
            _targetProperties = targetProperties;
        }

        public bool IsChanged
        {
            get => _isChanged;
            private set
            {
                if (_isChanged == value)
                    return;

                RaisePropertyChanging();

                _isChanged = value;

                RaisePropertyChanged();
            }
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }

        public void ReportChange([CallerMemberName] string propertyName = null)
        {
            if (!IsChanged && _targetProperties.Contains(propertyName))
                IsChanged = true;
        }

        void IObservableComponent.OwnerPropertyChanged<TValue>(TValue value, string propertyName)
        {
            ReportChange(propertyName);
        }
    }
}
