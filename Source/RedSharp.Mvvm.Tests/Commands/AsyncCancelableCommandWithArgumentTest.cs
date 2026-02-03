using System;
using System.ComponentModel;
using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Commands;

namespace RedSharp.Mvvm.Tests.Commands
{
    public class AsyncCancelableCommandWithArgumentTest
    {
        [Test]
        public void RaiseCanExecute_Call_CanExecuteChangedInvoked()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.CompletedTask);
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
            var command = new AsyncCancellableCommand<int>((argument, token) =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            });

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
            var command = new AsyncCancellableCommand<int>((argument, token) =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            }, argument => false);

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
            var command = new AsyncCancellableCommand<int>((argument, token) => throw new Exception());

            // Act
            // Assert
            Assert.DoesNotThrow(() => command.Execute(0));
        }

        //=======================================================================================================//

        [Test]
        public void Execute_CallFastCallback_IsRunningPropertyWasNotChanged()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.CompletedTask);
            var isRunningChanged = false;

            // Act
            command.PropertyChanged += (sender, argument) =>
            {
                if (argument.PropertyName == nameof(AsyncCommandBase.IsRunning))
                    isRunningChanged = true;
            };

            command.Execute(0);

            // Assert
            Assert.That(isRunningChanged, Is.False);
        }

        [Test]
        public void Execute_CallDelayedCallback_IsRunningPropertyWasNotChanged()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.Delay(AsyncCommandBase.AcceptableDelayTime * 2));
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

            command.Execute(0);

            // Assert
            Assert.That(isRunningChanged, Is.True);
            Assert.That(isRunningValue, Is.True);
        }

        //=======================================================================================================//

        [Test]
        public void CancelCommandCanExecute_ExecutionOfMainCommand_ReturnsTrue()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.Delay(AsyncCommandBase.AcceptableDelayTime * 20));
            var canExecute = false;

            // Act
            command.ExecuteAsync(0).Wait(AsyncCommandBase.AcceptableDelayTime);

            canExecute = command.CancelCommand.CanExecute(null);

            // Assert
            Assert.That(canExecute, Is.True);
        }

        [Test]
        public void CancelCommandCanExecute_NoExecutionOfMainCommand_ReturnsFalse()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.CompletedTask);
            var canExecute = false;

            // Act
            canExecute = command.CancelCommand.CanExecute(null);

            // Assert
            Assert.That(canExecute, Is.False);
        }

        [Test]
        public void CancelCommandCanExecute_AfterExecutionOfMainCommand_ReturnsFalse()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.CompletedTask);
            var canExecute = false;

            // Act
            command.Execute(0);
            canExecute = command.CancelCommand.CanExecute(null);

            // Assert
            Assert.That(canExecute, Is.False);
        }

        [Test]
        public void Execute_CallCancellation_IsCancellationRequested()
        {
            // Arrange
            var isCancellationRequested = false;
            var command = new AsyncCancellableCommand<int>(async (argument, token) =>
            {
                await Task.Delay(AsyncCommandBase.AcceptableDelayTime * 20);

                isCancellationRequested = token.IsCancellationRequested;
            });

            // Act
            var task = command.ExecuteAsync(0);
            
            task.Wait(AsyncCommandBase.AcceptableDelayTime);
            
            command.CancelCommand.Execute(null);

            task.Wait();

            // Assert
            Assert.That(isCancellationRequested, Is.True);
        }

        [Test]
        public void Execute_CallFastCallback_IsCancelingPropertyWasChanged()
        {
            // Arrange
            var command = new AsyncCancellableCommand<int>((argument, token) => Task.Delay(AsyncCommandBase.AcceptableDelayTime * 20, token));
            var isCancelingChanged = false;
            var isCancelingValue = false;
            var @delegate = default(PropertyChangedEventHandler);

            // Act
            @delegate = (sender, argument) =>
            {
                if (argument.PropertyName == nameof(AsyncCancellableCommandBase.IsCanceling))
                {
                    isCancelingChanged = true;
                    isCancelingValue = command.IsCanceling;

                    command.PropertyChanged -= @delegate;
                }
            };

            command.PropertyChanged += @delegate;

            var task = command.ExecuteAsync(0);
            
            task.Wait(AsyncCommandBase.AcceptableDelayTime);

            command.CancelCommand.Execute(null);

            task.Wait();

            // Assert
            Assert.That(isCancelingChanged, Is.True);
            Assert.That(isCancelingValue, Is.True);
        }

        //=======================================================================================================//

        [Test]
        public void Execute_CallWithCustomCanExecuteMatchCondition_CallbackShouldBeInvoked()
        {
            // Arrange
            var executionInvoked = false;
            var command = new AsyncCancellableCommand<int>((argument, token) =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            }, number => number < 100);

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
            var command = new AsyncCancellableCommand<int>((argument, token) =>
            {
                executionInvoked = true;

                return Task.CompletedTask;
            }, number => number < 100);

            // Act
            command.Execute(200);

            // Assert
            Assert.That(command.CanExecute(200), Is.False);
            Assert.That(executionInvoked, Is.False);
        }
    }
}
