using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Tests.Entities
{
    public class EditableObservableObjectTest
    {
        private class TestClass : EditableObservableObject
        {
            private int _trackableIntValue;
            private string _trackableStringValue;
            private int _privateTrackableIntValue;
            private string _privateTrackableStringValue;
            private int _nonTrackableIntValue;
            private string _nonTrackableStringValue;

            static TestClass()
            {
                BackupProperties<TestClass>(nameof(TrackableIntValue), nameof(TrackableStringValue));
            }

            public int TrackableIntValue
            {
                get => _trackableIntValue;
                set => CompareAndSetValue(ref _trackableIntValue, value);
            }

            public string TrackableStringValue
            {
                get => _trackableStringValue;
                set => CompareAndSetValue(ref _trackableStringValue, value);
            }

            public int PrivateTrackableIntValue
            {
                get => _privateTrackableIntValue;
                private set => CompareAndSetValue(ref _privateTrackableIntValue, value);
            }

            public string PrivateTrackableStringValue
            {
                get => _privateTrackableStringValue;
                private set => CompareAndSetValue(ref _privateTrackableStringValue, value);
            }

            public int NonTrackableIntValue
            {
                get => _nonTrackableIntValue;
                set => CompareAndSetValue(ref _nonTrackableIntValue, value);
            }

            public string NonTrackableStringValue
            {
                get => _nonTrackableStringValue;
                set => CompareAndSetValue(ref _nonTrackableStringValue, value);
            }

            public void ChangePrivateProperties(int first, string second)
            {
                PrivateTrackableIntValue = first;
                PrivateTrackableStringValue = second;
            }
        }

        [Test]
        public void BeginEditEndEdit_EditTrackablePropertiesObjectAndApply_ValuesAreApplied()
        {
            // Arrange
            var viewModel = new TestClass();

            const int expectedInt = 256;
            const string expectedString = "bla";

            // Act
            viewModel.BeginEdit();

            viewModel.TrackableIntValue = expectedInt;
            viewModel.TrackableStringValue = expectedString;

            viewModel.EndEdit();

            // Assert
            Assert.That(viewModel.TrackableIntValue, Is.EqualTo(expectedInt));
            Assert.That(viewModel.TrackableStringValue, Is.EqualTo(expectedString));
        }

        [Test]
        public void BeginEditCancelEdit_EditTrackablePropertiesObjectAndCancel_ValuesAreNotApplied()
        {
            // Arrange
            const int expectedInt = 256;
            const string expectedString = "bla";

            var viewModel = new TestClass()
            {
                TrackableIntValue = expectedInt,
                TrackableStringValue = expectedString
            };

            // Act
            viewModel.BeginEdit();

            viewModel.TrackableIntValue = 512;
            viewModel.TrackableStringValue = "blabla";

            viewModel.CancelEdit();

            // Assert
            Assert.That(viewModel.TrackableIntValue, Is.EqualTo(expectedInt));
            Assert.That(viewModel.TrackableStringValue, Is.EqualTo(expectedString));
        }

        [Test]
        public void BeginEditCancelEdit_EditPrivateTrackablePropertiesObjectAndCancel_ValuesAreNotApplied()
        {
            // Arrange
            const int expectedInt = 256;
            const string expectedString = "bla";

            var viewModel = new TestClass()
            {
                TrackableIntValue = expectedInt,
                TrackableStringValue = expectedString
            };

            // Act
            viewModel.BeginEdit();

            viewModel.ChangePrivateProperties(512, "blabla");

            viewModel.CancelEdit();

            // Assert
            Assert.That(viewModel.TrackableIntValue, Is.EqualTo(expectedInt));
            Assert.That(viewModel.TrackableStringValue, Is.EqualTo(expectedString));
        }

        [Test]
        public void CancelEdit_EditTrackablePropertiesWithoutBeginEdit_ValuesAreNotRestored()
        {
            // Arrange
            const int expectedInt = 256;
            const string expectedString = "bla";

            var viewModel = new TestClass();

            // Act
            viewModel.TrackableIntValue = expectedInt;
            viewModel.TrackableStringValue = expectedString;

            viewModel.CancelEdit();

            // Assert
            Assert.That(viewModel.TrackableIntValue, Is.EqualTo(expectedInt));
            Assert.That(viewModel.TrackableStringValue, Is.EqualTo(expectedString));
        }

        [Test]
        public void BeginEditCancelEdit_EditNonTrackablePropertiesObjectAndCancel_ValuesAreApplied()
        {
            // Arrange
            var viewModel = new TestClass();

            const int expectedInt = 256;
            const string expectedString = "bla";

            // Act
            viewModel.BeginEdit();

            viewModel.NonTrackableIntValue = expectedInt;
            viewModel.NonTrackableStringValue = expectedString;

            viewModel.CancelEdit();

            // Assert
            Assert.That(viewModel.NonTrackableIntValue, Is.EqualTo(expectedInt));
            Assert.That(viewModel.NonTrackableStringValue, Is.EqualTo(expectedString));
        }

        [Test]
        public void IsBeingEdited_CallBeginEditEndEdit_PropertyChangeValueAccordingly()
        {
            // Arrange
            var viewModel = new TestClass();
            var isBeingEditedBefore = false;
            var isBeingEditedDuring = false;
            var isBeingEditedAfter = false;

            // Act
            isBeingEditedBefore = viewModel.IsBeingEdited;

            viewModel.BeginEdit();

            isBeingEditedDuring = viewModel.IsBeingEdited;

            viewModel.EndEdit();

            isBeingEditedAfter = viewModel.IsBeingEdited;

            // Assert
            Assert.That(isBeingEditedBefore, Is.False);
            Assert.That(isBeingEditedDuring, Is.True);
            Assert.That(isBeingEditedAfter, Is.False);
        }

        [Test]
        public void IsBeingEdited_CallBeginEditCancelEdit_PropertyChangeValueAccordingly()
        {
            // Arrange
            var viewModel = new TestClass();
            var isBeingEditedBefore = false;
            var isBeingEditedDuring = false;
            var isBeingEditedAfter = false;

            // Act
            isBeingEditedBefore = viewModel.IsBeingEdited;

            viewModel.BeginEdit();

            isBeingEditedDuring = viewModel.IsBeingEdited;

            viewModel.CancelEdit();

            isBeingEditedAfter = viewModel.IsBeingEdited;

            // Assert
            Assert.That(isBeingEditedBefore, Is.False);
            Assert.That(isBeingEditedDuring, Is.True);
            Assert.That(isBeingEditedAfter, Is.False);
        }
    }
}
