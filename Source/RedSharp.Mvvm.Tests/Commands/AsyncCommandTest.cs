using System;
using System.ComponentModel;
using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Commands;

namespace RedSharp.Mvvm.Tests.Commands
{
    public class AsyncCommandTest
    {
        [Test]
        public void RaiseCanExecute_Call_CanExecuteChangedInvoked()
        {
            // Arrange
            var command = new AsyncCommand(() => Task.CompletedTask);
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
            var command = new AsyncCommand(() =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            });

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
            var command = new AsyncCommand(() =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            }, () => false);

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
            var command = new AsyncCommand(() => throw new Exception());

            // Act
            // Assert
            Assert.DoesNotThrow(command.Execute);
        }

        //=======================================================================================================//

        [Test]
        public void Execute_CallFastCallback_IsRunningPropertyWasNotChanged()
        {
            // Arrange
            var command = new AsyncCommand(() => Task.CompletedTask);
            var isRunningChanged = false;

            // Act
            command.PropertyChanged += (sender, argument) =>
            {
                if (argument.PropertyName == nameof(AsyncCommandBase.IsRunning))
                    isRunningChanged = true;
            };

            command.Execute();

            // Assert
            Assert.That(isRunningChanged, Is.False);
        }

        [Test]
        public void Execute_CallDelayedCallback_IsRunningPropertyWasNotChanged()
        {
            // Arrange
            var command = new AsyncCommand(() => Task.Delay(AsyncCommandBase.AcceptableDelayTime * 2));
            var isRunningChanged = false;
            var isRunningValue = false;
            var @delegate = default(PropertyChangedEventHandler);

            // Act
            @delegate = (sender, argument) =>
            {
                if (argument.PropertyName == nameof(AsyncCommandBase.IsRunning))
                {
                    isRunningChanged = true;
                    isRunningValue = command.IsRunning;

                    command.PropertyChanged -= @delegate;
                }
            };

            command.PropertyChanged += @delegate;

            command.Execute();

            // Assert
            Assert.That(isRunningChanged, Is.True);
            Assert.That(isRunningValue, Is.True);
        }
    }
}
