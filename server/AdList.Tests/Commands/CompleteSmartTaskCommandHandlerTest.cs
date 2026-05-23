using AdList.Application.Abstract;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Commands.CompleteSmartTaskCommand;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using AdList.Tests.Extensions;
using System.Security.Claims;

namespace AdList.Tests.Commands
{
    public class CompleteSmartTaskCommandHandlerTest
    {
        private readonly Mock<ISmartTaskRepository> _smartTaskRepository;
        private readonly CompleteSmartTaskCommandHandler _commandHandler;
        private readonly string Email = "test@test.com";
        private readonly ClaimsPrincipal _principal;

        public CompleteSmartTaskCommandHandlerTest()
        {
            _smartTaskRepository = new Mock<ISmartTaskRepository>();

            _smartTaskRepository.Setup(x => x.GetAll())
               .ReturnsCollectionOf(Enumerable.Range(1, 5).Select(i =>
               {
                   return new SmartTask
                   {
                       Id = i,
                       Author = Email,
                       Title = $"Task {i}",
                       Description = $"Description for Task {i}",
                       DueDate = DateTime.UtcNow.AddDays(7),
                       CompletionStatus = CompletionStatus.Pending,
                       CreatedAt = DateTime.UtcNow
                   };
               }));

            _principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Email, Email) }));

            _commandHandler = new CompleteSmartTaskCommandHandler(_smartTaskRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidPendingTask_CompletesTaskSuccessfully()
        {
            // Arrange
            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _smartTaskRepository.Verify(
                r => r.UpdateAsync(
                    It.Is<SmartTask>(t =>
                        t.Id == command.Id &&
                        t.CompletionStatus == CompletionStatus.Completed
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithValidPendingTask_SetsCompletionStatusToCompleted()
        {
            // Arrange
            SmartTask? capturedTask = null;
            _smartTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()))
                .Callback<SmartTask, CancellationToken>((task, ct) => capturedTask = task)
                .Returns(ValueTask.CompletedTask);

            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(CompletionStatus.Completed, capturedTask.CompletionStatus);
        }

        [Fact]
        public async Task Handle_WithNonExistentTask_ThrowsException()
        {
            // Arrange
            var command = new CompleteSmartTaskCommand
            {
                Id = 999,
                CurrentPrincipal = _principal
            };

            // Assert
            await Assert.ThrowsAsync<DomainException>(
                async () => await _commandHandler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_WithTaskOwnedByDifferentUser_ThrowsException()
        {
            // Arrange
            _smartTaskRepository.Setup(x => x.GetAll())
                .ReturnsCollectionOf(
                    new SmartTask
                    {
                        Id = 1,
                        Author = Email,
                        Title = "Task 1",
                        Description = "Description for Task 1",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SmartTask
                    {
                        Id = 2,
                        Author = "different@test.com",
                        Title = "Task 2",
                        Description = "Description for Task 2",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                );

            var command = new CompleteSmartTaskCommand
            {
                Id = 2,
                CurrentPrincipal = _principal
            };

            // Assert
            await Assert.ThrowsAsync<DomainException>(
                async () => await _commandHandler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_WithAlreadyCompletedTask_ThrowsException()
        {
            // Arrange
            _smartTaskRepository.Setup(x => x.GetAll())
                .ReturnsCollectionOf(
                    new SmartTask
                    {
                        Id = 1,
                        Author = Email,
                        Title = "Completed Task",
                        Description = "Already completed",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Completed,
                        CreatedAt = DateTime.UtcNow
                    }
                );

            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Assert
            await Assert.ThrowsAsync<DomainException>(
                async () => await _commandHandler.Handle(command, CancellationToken.None)
            );
        }

        [Fact]
        public async Task Handle_WithCancellationToken_PassesTokenToRepository()
        {
            // Arrange
            var cancellationToken = new CancellationToken();
            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            await _commandHandler.Handle(command, cancellationToken);

            // Assert
            _smartTaskRepository.Verify(
                r => r.UpdateAsync(It.IsAny<SmartTask>(), cancellationToken),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithMultiplePendingTasks_CompletesCorrectTask()
        {
            // Arrange
            _smartTaskRepository.Setup(x => x.GetAll())
                .ReturnsCollectionOf(
                    new SmartTask
                    {
                        Id = 1,
                        Author = Email,
                        Title = "Task 1",
                        Description = "Description 1",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    },
                    new SmartTask
                    {
                        Id = 2,
                        Author = Email,
                        Title = "Task 2",
                        Description = "Description 2",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                );

            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _smartTaskRepository.Verify(
                r => r.UpdateAsync(It.Is<SmartTask>(t => t.Id == 1), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_ReturnsEmptyResponse()
        {
            // Arrange
            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EmptyResponse.Instance, result);
        }

        [Fact]
        public async Task Handle_DoesNotModifyOtherTaskProperties()
        {
            // Arrange
            SmartTask? capturedTask = null;
            var originalTitle = "Original Title";
            var originalDescription = "Original Description";
            var originalDueDate = DateTime.UtcNow.AddDays(7);

            _smartTaskRepository.Setup(x => x.GetAll())
                .ReturnsCollectionOf(
                    new SmartTask
                    {
                        Id = 1,
                        Author = Email,
                        Title = originalTitle,
                        Description = originalDescription,
                        DueDate = originalDueDate,
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                );

            _smartTaskRepository.Setup(r => r.UpdateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()))
                .Callback<SmartTask, CancellationToken>((task, ct) => capturedTask = task)
                .Returns(ValueTask.CompletedTask);

            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(originalTitle, capturedTask.Title);
            Assert.Equal(originalDescription, capturedTask.Description);
            Assert.Equal(originalDueDate, capturedTask.DueDate);
            Assert.Equal(Email, capturedTask.Author);
        }

        [Fact]
        public async Task Handle_WithValidTask_CallsUpdateAsyncOnce()
        {
            // Arrange
            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _smartTaskRepository.Verify(
                r => r.UpdateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithInvalidTask_DoesNotCallUpdateAsync()
        {
            // Arrange
            var command = new CompleteSmartTaskCommand
            {
                Id = 999, // Non-existent task
                CurrentPrincipal = _principal
            };

            // Act & Assert
            await Assert.ThrowsAsync<DomainException>(
                async () => await _commandHandler.Handle(command, CancellationToken.None)
            );

            _smartTaskRepository.Verify(
                r => r.UpdateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_ExtractsEmailFromPrincipal()
        {
            // Arrange
            var differentEmail = "different@test.com";
            var differentPrincipal = new ClaimsPrincipal(
                new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Email, differentEmail) })
            );

            _smartTaskRepository.Setup(x => x.GetAll())
                .ReturnsCollectionOf(
                    new SmartTask
                    {
                        Id = 1,
                        Author = differentEmail,
                        Title = "Task 1",
                        Description = "Description 1",
                        DueDate = DateTime.UtcNow.AddDays(7),
                        CompletionStatus = CompletionStatus.Pending,
                        CreatedAt = DateTime.UtcNow
                    }
                );

            var command = new CompleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = differentPrincipal
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _smartTaskRepository.Verify(
                r => r.UpdateAsync(
                    It.Is<SmartTask>(t => t.Author == differentEmail),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }
    }
}
