using System;
using Newtonsoft.Json.Linq;
using RedSharp.Mvvm.Bindings.Entities;

namespace RedSharp.Mvvm.Bindings.Tests.Entities
{
    public class ConverterChainTests
    {
        [Test]
        public void Value_SetValueWithMultiplierConverter_EqualsExpected()
        {
            // Arrange
            var fixedChain = new FixedValueChain<int>();
            var testChain = new ConverterChain<int, int>(number => number * 2);

            const int inputValue = 256;
            const int expectedValue = inputValue * 2;

            // Act
            fixedChain.ForceUpdateTarget(inputValue);
            testChain.TryUpdateTarget(fixedChain);

            // Asserts
            Assert.That(testChain.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void ValueChangedEvent_ForceUpdateOnTheFirstChain_IsNotInvoked()
        {
            // Arrange
            var fixedChain = new FixedValueChain<int>();
            var testChain = new ConverterChain<int, int>(number => number * 2);
            var valueChangedInvoked = false;

            // Act
            testChain.ValueChanged += (sender, arguments) => valueChangedInvoked = true;

            fixedChain.ForceUpdateTarget(256);
            testChain.TryUpdateTarget(fixedChain);

            // Asserts
            Assert.That(valueChangedInvoked, Is.False);
        }

        [Test]
        public void Value_TryToSetDirectly_ThrowsException()
        {
            // Arrange
            var testChain = new ConverterChain<int, int>(number => number * 2);

            // Act
            // Asserts
            Assert.Throws<NotSupportedException>(() => testChain.Value = 256);
        }
    }
}
