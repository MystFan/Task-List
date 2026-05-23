using AdList.Application.Abstract;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Commands.CreateSmartTaskCommand;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using AdList.Tests.Extensions;
using System.Security.Claims;

namespace AdList.Tests.Commands
{
    public class CreateSmartTaskCommandHandlerTest
    {
        private readonly Mock<ISmartTaskRepository> _smartTaskRepository;
        private readonly Mock<IRepository<ApplicationUser>> _applicationUserRepository;
        private readonly CreateSmartTaskCommandHandler _commandHandler;
        private const string TestEmail = "test@example.com";
        private readonly ClaimsPrincipal _testPrincipal;

        public CreateSmartTaskCommandHandlerTest()
        {
            _smartTaskRepository = new Mock<ISmartTaskRepository>();
            _applicationUserRepository = new Mock<IRepository<ApplicationUser>>();
            _commandHandler = new CreateSmartTaskCommandHandler(
                _smartTaskRepository.Object,
                _applicationUserRepository.Object
            );

            _applicationUserRepository.Setup(x => x.GetAll())
               .ReturnsCollectionOf(new ApplicationUser
               {
                   Email = TestEmail,
                   Name = "Test User"
               });

            // Setup test principal with email claim
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, TestEmail)
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _testPrincipal = new ClaimsPrincipal(identity);
        }

        [Fact]
        public async Task Handle_WithValidUser_CreatesTaskSuccessfully()
        {
            // Arrange
            var user = new ApplicationUser
            {
                Id = 1,
                Email = TestEmail,
                Name = "Test User"
            };

            var command = new CreateSmartTaskCommand {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(EmptyResponse.Instance, result);

            _smartTaskRepository.Verify(
                r => r.CreateAsync(
                    It.Is<SmartTask>(t =>
                        t.Author == TestEmail &&
                        t.Title == "Test Task" &&
                        t.Description == "Test Description" &&
                        t.CompletionStatus == CompletionStatus.Pending
                    ),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithUserNotFound_ThrowsDomainException()
        {
            // Arrange
            _applicationUserRepository.Setup(r => r.GetAll()).ReturnsCollectionOf();

            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            Func<Task> act = async () => await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            DomainException domainException = await Assert.ThrowsAsync<DomainException>(() => _commandHandler.Handle(command, CancellationToken.None));
            Assert.Equal(ExceptionReasonCode.UserNotFound, domainException.ReasonCode);

            _smartTaskRepository.Verify(
                r => r.CreateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()),
                Times.Never
            );
        }

        [Fact]
        public async Task Handle_SetsCorrectTaskProperties()
        {
            // Arrange
            SmartTask? capturedTask = null;
            _smartTaskRepository.Setup(r => r.CreateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()))
                .Callback<SmartTask, CancellationToken>((task, ct) => capturedTask = task)
                .Returns(ValueTask.CompletedTask);

            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(TestEmail, capturedTask.Author);
            Assert.Equal("Test Task", capturedTask.Title);
            Assert.Equal("Test Description", capturedTask.Description);
            Assert.Equal(command.DueDate, capturedTask.DueDate);
            Assert.Equal(CompletionStatus.Pending, capturedTask.CompletionStatus);
        }

        [Fact]
        public async Task Handle_WithNullDescription_CreatesTaskSuccessfully()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = null,
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(EmptyResponse.Instance, result);

            _smartTaskRepository.Verify(
                r => r.CreateAsync(
                    It.Is<SmartTask>(t => t.Description == null),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_WithNullDueDate_CreatesTaskSuccessfully()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = null
            };

            // Act
            var result = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Equal(EmptyResponse.Instance, result);

            _smartTaskRepository.Verify(
                r => r.CreateAsync(
                    It.Is<SmartTask>(t => t.DueDate == null),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Fact]
        public async Task Handle_AlwaysSetsCompletionStatusToPending()
        {
            // Arrange
            SmartTask? capturedTask = null;
            _smartTaskRepository.Setup(r => r.CreateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()))
                .Callback<SmartTask, CancellationToken>((task, ct) => capturedTask = task)
                .Returns(ValueTask.CompletedTask);

            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(CompletionStatus.Pending, capturedTask.CompletionStatus);
        }

        [Fact]
        public async Task Handle_WithMultipleUsers_FindsCorrectUser()
        {
            // Arrange
            var users = new List<ApplicationUser>
            {
                new ApplicationUser { Id = 1, Email = "user1@example.com", Name = "User 1" },
                new ApplicationUser { Id = 2, Email = TestEmail, Name = "Test User" },
                new ApplicationUser { Id = 3, Email = "user3@example.com", Name = "User 3" }
            };

            _applicationUserRepository.Setup(r => r.GetAll())
                .ReturnsCollectionOf(users);

            SmartTask? capturedTask = null;
            _smartTaskRepository.Setup(r => r.CreateAsync(It.IsAny<SmartTask>(), It.IsAny<CancellationToken>()))
                .Callback<SmartTask, CancellationToken>((task, ct) => capturedTask = task)
                .Returns(ValueTask.CompletedTask);

            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Test Task",
                Description = "Test Description",
                DueDate = DateTime.UtcNow.AddDays(7)
            };

            // Act
            await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(capturedTask);
            Assert.Equal(TestEmail, capturedTask.Author);
        }
    }
}
