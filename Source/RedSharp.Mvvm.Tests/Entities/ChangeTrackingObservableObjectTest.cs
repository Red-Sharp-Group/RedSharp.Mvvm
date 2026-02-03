using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Tests.Entities
{
    public class ChangeTrackingObservableObjectTest
    {
        private class TestClass : ChangeTrackingObservableObject
        {
            private int _trackableIntValue;
            private string _trackableStringValue;
            private int _nonTrackableIntValue;
            private string _nonTrackableStringValue;

            static TestClass()
            {
                TrackProperties<TestClass>(nameof(TrackableIntValue), nameof(TrackableStringValue));
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
        }

        [Test]
        public void IsChanged_ChangeTrackableProperties_IsTrue()
        {
            // Arrange
            var viewModel = new TestClass();

            // Act
            viewModel.TrackableIntValue = 1;
            viewModel.TrackableStringValue = "bla";

            // Assert
            Assert.That(viewModel.IsChanged, Is.True);
        }

        [Test]
        public void IsChanged_ChangeNonTrackableProperties_IsFalse()
        {
            // Arrange
            var viewModel = new TestClass();

            // Act
            viewModel.NonTrackableIntValue = 1;
            viewModel.NonTrackableStringValue = "bla";

            // Assert
            Assert.That(viewModel.IsChanged, Is.False);
        }

        [Test]
        public void IsChanged_ChangeTrackableAndNonTrackableProperties_IsTrue()
        {
            // Arrange
            var viewModel = new TestClass();

            // Act
            viewModel.TrackableIntValue = 1;
            viewModel.TrackableStringValue = "bla";
            viewModel.NonTrackableIntValue = 1;
            viewModel.NonTrackableStringValue = "bla";

            // Assert
            Assert.That(viewModel.IsChanged, Is.True);
        }

        [Test]
        public void IsChanged_ChangeTrackablePropertiesAndAccept_IsFalse()
        {
            // Arrange
            var viewModel = new TestClass();

            // Act
            viewModel.TrackableIntValue = 1;
            viewModel.TrackableStringValue = "bla";

            viewModel.AcceptChanges();

            // Assert
            Assert.That(viewModel.IsChanged, Is.False);
        }
    }
}
