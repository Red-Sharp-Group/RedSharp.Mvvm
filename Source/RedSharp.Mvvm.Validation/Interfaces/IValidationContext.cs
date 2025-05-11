using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Validation.Interfaces
{
    public interface IValidationContext
    {
        ObservableObject Owner { get; }

        object MakeValidationInfoObject(string property, string rule, bool isRunning, bool IsValid, object stored);

        void ReportCountChanged();

        void ReportValidationChanged(string property);
    }
}
