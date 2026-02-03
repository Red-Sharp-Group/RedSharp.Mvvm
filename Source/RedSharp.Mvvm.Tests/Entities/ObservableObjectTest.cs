using RedSharp.Mvvm.Abstracts;

namespace RedSharp.Mvvm.Tests.Entities
{
    public class ObservableObjectTest
    {
        private class TestClass : ObservableObject
        {
            private int _int;
            private float _float;
            private double _double;
            private string _ignoreCaseString;

            public int Int
            {
                get => _int;
                set => CompareAndSetValue(ref _int, value);
            }

            public float Float
            {
                get => _float;
                set => CompareAndSetValueWithPrecision(ref _float, value);
            }

            public double Double
            {
                get => _double;
                set => CompareAndSetValueWithPrecision(ref _double, value);
            }

            public string IgnoreCaseString
            {
                get => _ignoreCaseString;
                set => CompareAndSetStringValue(ref _ignoreCaseString, value, System.StringComparison.InvariantCultureIgnoreCase);
            }
        }

        //=======================================================================================================//

        [Test]
        public void CompareAndSetValueWithPrecision_ChangeDoubleValueWithPrecisionRange_PropertyChangingInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => count++;
            viewModel.Double = 0.1d;
            viewModel.Double = viewModel.Double + (ObservableObject.DefaultDoubleValuePrecision / 2.0d);

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void CompareAndSetValueWithPrecision_ChangeDoubleValueWithPrecisionRange_PropertyChangedInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => count++;
            viewModel.Double = 0.1d;
            viewModel.Double = viewModel.Double + (ObservableObject.DefaultDoubleValuePrecision / 2.0d);

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        //=======================================================================================================//

        [Test]
        public void CompareAndSetValueWithPrecision_ChangeFloatValueWithPrecisionRange_PropertyChangingInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => count++;
            viewModel.Float = 0.1f;
            viewModel.Float = viewModel.Float + (ObservableObject.DefaultFloatValuePrecision / 2.0f);

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        [Test]
        public void CompareAndSetValueWithPrecision_ChangeFloatValueWithPrecisionRange_PropertyChangedInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => count++;
            viewModel.Float = 0.1f;
            viewModel.Float = viewModel.Float + (ObservableObject.DefaultFloatValuePrecision / 2.0f);

            // Assert
            Assert.That(count, Is.EqualTo(1));
        }

        //=======================================================================================================//

        [Test]
        public void CompareAndSetStringValue_ChangeIgnoreCaseStringValueWithDifferentCase_PropertyChangingInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => count++;
            viewModel.IgnoreCaseString = "abc";
            viewModel.IgnoreCaseString = "ABC";

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }

        [Test]
        public void CompareAndSetStringValue_ChangeIgnoreCaseStringValueWithDifferentCase_PropertyChangedInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => count++;
            viewModel.IgnoreCaseString = "abc";
            viewModel.IgnoreCaseString = "ABC";

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }

        //=======================================================================================================//

        [Test]
        public void CompareAndSetValue_ChangeIntValue_PropertyChangingInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => count++;
            viewModel.Int = 100;

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }

        [Test]
        public void CompareAndSetValue_ChangeIntValueTwiceWithSameValue_PropertyChangingInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => count++;
            viewModel.Int = 100;
            viewModel.Int = 100;

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }


        [Test]
        public void CompareAndSetValue_ChangeIntValue_PropertyChangingInvokedBeforeValueChanged()
        {
            // Arrange
            var viewModel = new TestClass();
            var result = false;

            // Act
            viewModel.PropertyChanging += (sender, arguments) => result = viewModel.Int == 0;
            viewModel.Int = 100;

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CompareAndSetValue_ChangeIntValue_PropertyChangedInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => count++;
            viewModel.Int = 100;

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }

        [Test]
        public void CompareAndSetValue_ChangeIntValueTwiceWithSameValue_PropertyChangedInvokedOnce()
        {
            // Arrange
            var viewModel = new TestClass();
            var count = 0;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => count++;
            viewModel.Int = 100;
            viewModel.Int = 100;

            // Assert
            Assert.That(count, Is.EqualTo(1));            
        }

        [Test]
        public void CompareAndSetValue_ChangeIntValue_PropertyChangedInvokedAfterValueChanged()
        {
            // Arrange
            var viewModel = new TestClass();
            var result = false;

            // Act
            viewModel.PropertyChanged += (sender, arguments) => result = viewModel.Int == 0;
            viewModel.Int = 100;

            // Assert
            Assert.That(result, Is.False);
        }
    }
}