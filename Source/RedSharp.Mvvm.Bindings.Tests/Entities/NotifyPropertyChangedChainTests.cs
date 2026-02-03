using System;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Bindings.Entities;

namespace RedSharp.Mvvm.Bindings.Tests.Entities
{
    public class NotifyPropertyChangedChainTests
    {
        private class TestClass : ObservableObject
        {
            private int _intValue;
            private string _stringValue;

            public int IntValue
            {
                get => _intValue;
                set => CompareAndSetValue(ref _intValue, value);
            }

            public string StringValue
            {
                get => _stringValue;
                set => CompareAndSetValue(ref _stringValue, value);
            }
        }

        [Test]
        public void ValueChangedEvent_SetValueOnTargetObject_EventInvoked()
        {
            // Arrange
            var target = new TestClass();
            var fixedChain = new FixedValueChain<TestClass>();
            var testChain = new NotifyPropertyChangedChain<TestClass, int>(nameof(TestClass.IntValue), target => target.IntValue);
            var valueChangedInvoked = false;

            const int expectedValue = 256;

            // Act
            fixedChain.ForceUpdateTarget(target);
            testChain.TryUpdateTarget(fixedChain);

            testChain.ValueChanged += (sender, arguments) => valueChangedInvoked = true;

            target.IntValue = expectedValue;

            // Asserts
            Assert.That(valueChangedInvoked, Is.True);
        }

        [Test]
        public void ValueChangedEvent_SetValueOnBinding_Invoked()
        {
            // Arrange
            var target = new TestClass();
            var fixedChain = new FixedValueChain<TestClass>();
            var testChain = new NotifyPropertyChangedChain<TestClass, int>(nameof(TestClass.IntValue), target => target.IntValue, (target, value) => target.IntValue = value);
            var valueChangedInvoked = false;

            const int expectedValue = 256;

            // Act
            fixedChain.ForceUpdateTarget(target);
            testChain.TryUpdateTarget(fixedChain);

            testChain.ValueChanged += (sender, arguments) => valueChangedInvoked = true;

            testChain.Value = expectedValue;

            // Asserts
            Assert.That(valueChangedInvoked, Is.True);
        }

        [Test]
        public void Value_ChangePropertyOnTargetObject_ValueEqualsExpected()
        {
            // Arrange
            var target = new TestClass();
            var fixedChain = new FixedValueChain<TestClass>();
            var testChain = new NotifyPropertyChangedChain<TestClass, int>(nameof(TestClass), target => target.IntValue);

            const int expectedValue = 256;

            // Act
            fixedChain.ForceUpdateTarget(target);
            testChain.TryUpdateTarget(fixedChain);

            target.IntValue = expectedValue;

            // Asserts
            Assert.That(testChain.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Value_SetValueOnBinding_ValueOnTargetEqualsExpected()
        {
            // Arrange
            var target = new TestClass();
            var fixedChain = new FixedValueChain<TestClass>();
            var testChain = new NotifyPropertyChangedChain<TestClass, int>(nameof(TestClass), target => target.IntValue, (target, value) => target.IntValue = value);

            const int expectedValue = 256;

            // Act
            fixedChain.ForceUpdateTarget(target);
            testChain.TryUpdateTarget(fixedChain);

            testChain.Value = expectedValue;

            // Asserts
            Assert.That(target.IntValue, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Value_SetValueOnBindingWithoutSetter_ThrowsException()
        {
            // Arrange
            var target = new TestClass();
            var fixedChain = new FixedValueChain<TestClass>();
            var testChain = new NotifyPropertyChangedChain<TestClass, int>(nameof(TestClass), target => target.IntValue);

            // Act
            fixedChain.ForceUpdateTarget(target);
            testChain.TryUpdateTarget(fixedChain);

            // Asserts
            Assert.Throws<NotSupportedException>(() => testChain.Value = 256);
        }
    }
}
