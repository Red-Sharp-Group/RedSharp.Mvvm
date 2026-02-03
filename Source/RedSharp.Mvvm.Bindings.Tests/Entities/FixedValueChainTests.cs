using System;
using RedSharp.Mvvm.Bindings.Entities;

namespace RedSharp.Mvvm.Bindings.Tests.Entities
{
    public class FixedValueChainTests
    {
        [Test]
        public void ForceUpdateTarget_Call_ValueChangedEventInvoked() 
        {
            // Arrange
            var chain = new FixedValueChain<int>();
            var valueChangedInvoked = false;

            const int expectedValue = 256;

            // Act
            chain.ValueChanged += (sender, arguments) => valueChangedInvoked = true;

            chain.ForceUpdateTarget(expectedValue);

            // Asserts
            Assert.That(valueChangedInvoked, Is.True);
            Assert.That(chain.Value, Is.EqualTo(expectedValue));
        }

        [Test]
        public void Value_TryToSetDirectly_ThrowsException()
        {
            // Arrange
            var chain = new FixedValueChain<int>();

            // Act
            // Asserts
            Assert.Throws<NotSupportedException>(() => chain.Value = 256);
        }
    }
}
