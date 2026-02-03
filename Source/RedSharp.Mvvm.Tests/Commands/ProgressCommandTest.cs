using System;
using System.ComponentModel;
using System.Threading.Tasks;
using RedSharp.Mvvm.Abstracts;
using RedSharp.Mvvm.Commands;

namespace RedSharp.Mvvm.Tests.Commands
{
    public class ProgressCommandTest
    {
        [Test]
        public void RaiseCanExecute_Call_CanExecuteChangedInvoked()
        {
            // Arrange
            var command = new ProgressCommand(progress => Task.CompletedTask);
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
            var command = new ProgressCommand(progress =>
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
            var command = new ProgressCommand(progress =>
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
            var command = new ProgressCommand(progress => throw new Exception());

            // Act
            // Assert
            Assert.DoesNotThrow(command.Execute);
        }

        //=======================================================================================================//

        [Test]
        public void Execute_CallFastCallback_IsRunningPropertyWasNotChanged()
        {
            // Arrange
            var command = new ProgressCommand(progress => Task.CompletedTask);
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
            var command = new ProgressCommand(progress => Task.Delay(AsyncCommandBase.AcceptableDelayTime * 2));
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

        //=======================================================================================================//

        [Test]
        public void Execute_CallWithProgressChange_ProgressPropertyWasChanged()
        {
            // Arrange
            const double targetValue = 0.5;

            var command = new ProgressCommand(progress => 
            {
                progress.Report(targetValue);

                return Task.Delay(AsyncCommandBase.AcceptableDelayTime * 20);
            });
            var progressChanged = false;
            var initialValue = 0.0;
            var reportedValue = 0.0;
            var finalValue = 0.0;
            var @delegate = default(PropertyChangedEventHandler);

            // Act
            @delegate = (sender, arguments) =>
            {
                if (arguments.PropertyName == nameof(ProgressCommandBase<object>.Progress))
                {
                    progressChanged = true;
                    reportedValue = command.Progress;

                    command.PropertyChanged -= @delegate;
                }
            };

            command.PropertyChanged += @delegate;

            initialValue = command.Progress;

            command.Execute();

            finalValue = command.Progress;

            // Assert
            Assert.That(initialValue, Is.EqualTo(0.0));
            Assert.That(progressChanged, Is.True);
            Assert.That(reportedValue, Is.EqualTo(targetValue));
            Assert.That(finalValue, Is.EqualTo(0.0));
        }
    }
}
