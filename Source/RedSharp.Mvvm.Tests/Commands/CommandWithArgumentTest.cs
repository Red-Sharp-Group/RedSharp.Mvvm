using System;
using RedSharp.Mvvm.Commands;

namespace RedSharp.Mvvm.Tests.Commands
{
    public class CommandWithArgumentTest
    {
        [Test]
        public void RaiseCanExecute_Call_CanExecuteChangedInvoked()
        {
            // Arrange
            var command = new Command<int>(number => { });
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
            var command = new Command<int>(number => executionInvoked = true);

            // Act
            command.Execute(0);

            // Assert
            Assert.That(command.CanExecute(0), Is.True);
            Assert.That(executionInvoked, Is.True);
        }

        [Test]
        public void Execute_CallWithFalseCanExecute_CallbackIsNotInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new Command<int>(number => executionInvoked = true, number => false);

            // Act
            command.Execute(0);

            // Assert
            Assert.That(command.CanExecute(0), Is.False);
            Assert.That(executionInvoked, Is.False);
        }

        [Test]
        public void Execute_CallThrowsException_NoExceptionOutside()
        {
            // Arrange
            var command = new Command<int>(number => throw new Exception());

            // Act
            // Assert
            Assert.DoesNotThrow(() => command.Execute(0));
        }

        //=======================================================================================================//

        [Test]
        public void Execute_CallWithCustomCanExecuteMatchCondition_CallbackShouldBeInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new Command<int>(number => executionInvoked = true, number => number < 100);

            // Act
            command.Execute(0);

            // Assert
            Assert.That(command.CanExecute(0), Is.True);
            Assert.That(executionInvoked, Is.True);
        }

        [Test]
        public void Execute_CallWithCustomCanExecuteDoesNotMatchCondition_CallbackShouldNotBeInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new Command<int>(number => executionInvoked = true, number => number < 100);

            // Act
            command.Execute(200);

            // Assert
            Assert.That(command.CanExecute(200), Is.False);
            Assert.That(executionInvoked, Is.False);
        }
    }
}
