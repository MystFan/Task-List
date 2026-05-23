using AdList.Application.Abstract;
using AdList.Application.Commands.DeleteSmartTaskCommand;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using AdList.Tests.Extensions;
using System.Security.Claims;

namespace AdList.Tests.Commands
{
    public class DeleteSmartTaskCommandHandlerTest
    {
        private readonly Mock<ISmartTaskRepository> _smartTaskRepository;
        private readonly DeleteSmartTaskCommandHandler _commandHandler;
        private readonly string Email = "test@test.com";
        private readonly ClaimsPrincipal _principal;

        public DeleteSmartTaskCommandHandlerTest()
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
                       DueDate = DateTime.UtcNow,
                       CompletionStatus = CompletionStatus.Pending,
                       CreatedAt = DateTime.UtcNow
                   };
               }));

            _principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Email, Email) }));

            _commandHandler = new DeleteSmartTaskCommandHandler(_smartTaskRepository.Object);
        }

        [Fact]
        public async Task Handle_WithValidTaskAndAuthor_DeletesTaskSuccessfully()
        {
            // Arrange
            var command = new DeleteSmartTaskCommand
            {
                Id = 1,
                CurrentPrincipal = _principal
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            _smartTaskRepository.Verify(
                r => r.DeleteAsync(It.Is<SmartTask>(t => t.Id == command.Id), It.IsAny<CancellationToken>()),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithNonExistentTask_ThrowsException()
        {
            // Arrange
            var command = new DeleteSmartTaskCommand
            {
                Id = 999,
                CurrentPrincipal = _principal
            };

            // Assert
            await Assert.ThrowsAsync<DomainException>(async () => await _commandHandler.Handle(command, CancellationToken.None));
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
                        DueDate = DateTime.UtcNow,
                        CompletionStatus = CompletionStatus.Pending,
                    },
                    new SmartTask
                    {
                        Id = 2,
                        Author = "test1@test.com",
                        Title = $"Task 1",
                        Description = $"Description for Task 1",
                        DueDate = DateTime.UtcNow,
                        CompletionStatus = CompletionStatus.Pending,
                    }
                );

            // Arrange
            var command = new DeleteSmartTaskCommand
            {
                Id = 2,
                CurrentPrincipal = _principal
            };

            // Assert
            await Assert.ThrowsAsync<DomainException>(async () => await _commandHandler.Handle(command, CancellationToken.None));
        }
    }
}
