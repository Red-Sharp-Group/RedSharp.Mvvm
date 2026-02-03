using System;
using RedSharp.Mvvm.Commands;

namespace RedSharp.Mvvm.Tests.Commands
{
    public class CommandTest
    {
        [Test]
        public void RaiseCanExecute_Call_CanExecuteChangedInvoked()
        {
            // Arrange
            var command = new Command(() => { });
            var eventInvoked = false;

            // Act
            command.CanExecuteChanged += (sender, arguments) => eventInvoked = true;
            command.RaiseCanExecute();

            // Assert
            Assert.That(eventInvoked, Is.True);
        }

        [Test]
        public void Execute_CallWithDefaultCanExecute_CallbackShouldBeInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new Command(() => executionInvoked = true);

            // Act
            command.Execute();

            // Assert
            Assert.That(command.CanExecute(), Is.True);
            Assert.That(executionInvoked, Is.True);
        }

        [Test]
        public void Execute_CallWithFalseCanExecute_CallbackIsNotInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new Command(() => executionInvoked = true, () => false);

            // Act
            command.Execute();

            // Assert
            Assert.That(command.CanExecute(), Is.False);
            Assert.That(executionInvoked, Is.False);
        }

        [Test]
        public void Execute_CallThrowsException_NoExceptionOutside()
        {
            // Arrange
            var command = new Command(() => throw new Exception());

            // Act
            // Assert
            Assert.DoesNotThrow(command.Execute);
        }
    }
}
