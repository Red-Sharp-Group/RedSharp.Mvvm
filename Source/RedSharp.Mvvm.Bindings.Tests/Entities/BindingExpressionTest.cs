using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Bindings.Helpers;
using RedSharp.Mvvm.Bindings.Interfaces;

namespace RedSharp.Mvvm.Bindings.Tests.Entities
{
    public class BindingExpressionTest
    {
        private class FirstTestClass : ObservableObject
        {
            private SecondTestClass _secondTestClass;

            public SecondTestClass SecondObject
            {
                get => _secondTestClass;
                set => CompareAndSetValue(ref _secondTestClass, value);
            }
        }

        private class SecondTestClass : ObservableObject
        {
            private FinalTestClass _finalTestClass;

            public FinalTestClass FinalObject
            {
                get => _finalTestClass;
                set => CompareAndSetValue(ref _finalTestClass, value);
            }
        }

        private class FinalTestClass : ObservableObject
        {
            private int _intValue;

            public int IntValue
            {
                get => _intValue;
                set => CompareAndSetValue(ref _intValue, value);
            }
        }

        [Test]
        public void Value_InitExpression_InitialValueIsEqualToExpected()
        {
            // Arrange
            const int expectedValue = 256;

            var testClass = new FirstTestClass()
            {
                SecondObject = new SecondTestClass()
                {
                    FinalObject = new FinalTestClass()
                    {
                        IntValue = expectedValue
                    }
                }
            };

            var bindingExpression = default(IBindingExpression<int>);

            // Act
            bindingExpression = testClass.Bind(item => item.SecondObject)
                                         .Bind(item => item.FinalObject)
                                         .Bind(item => item.IntValue)
                                         .CompleteExpression();

            // Assert
            Assert.That(bindingExpression.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Value_ChangeLastValue_IsEqualToExpected()
        {
            // Arrange
            const int initialValue = 256;
            const int expectedValue = 512;

            var testClass = new FirstTestClass()
            {
                SecondObject = new SecondTestClass()
                {
                    FinalObject = new FinalTestClass()
                    {
                        IntValue = initialValue
                    }
                }
            };

            var bindingExpression = default(IBindingExpression<int>);

            // Act
            bindingExpression = testClass.Bind(item => item.SecondObject)
                                         .Bind(item => item.FinalObject)
                                         .Bind(item => item.IntValue)
                                         .CompleteExpression();

            testClass.SecondObject.FinalObject.IntValue = expectedValue;

            // Assert
            Assert.That(bindingExpression.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Value_ChangeMiddleValue_IsEqualToExpected()
        {
            // Arrange
            const int initialValue = 256;
            const int expectedValue = 512;

            var testClass = new FirstTestClass()
            {
                SecondObject = new SecondTestClass()
                {
                    FinalObject = new FinalTestClass()
                    {
                        IntValue = initialValue
                    }
                }
            };

            var finalObject = new FinalTestClass()
            {
                IntValue = expectedValue
            };

            var bindingExpression = default(IBindingExpression<int>);

            // Act
            bindingExpression = testClass.Bind(item => item.SecondObject)
                                         .Bind(item => item.FinalObject)
                                         .Bind(item => item.IntValue)
                                         .CompleteExpression();

            testClass.SecondObject.FinalObject = finalObject;

            // Assert
            Assert.That(bindingExpression.Value, Is.EqualTo(expectedValue));
        }
    }
}
