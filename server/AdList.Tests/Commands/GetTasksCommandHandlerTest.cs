using AdList.Application.Abstract;
using AdList.Application.Commands.GetTasksCommand;
using AdList.Domain.Entities;
using AdList.Tests.Extensions;
using System.Linq.Expressions;
using System.Security.Claims;

namespace AdList.Tests.Commands
{
    public class GetTasksCommandHandlerTest
    {
        private GetTasksCommandHandler _commandHandler = null!;
        private Mock<ISmartTaskRepository> _smartTaskRepository = null!;
        private Mock<IRepository<ApplicationUser>> _applicationUserRepository = null!;
        private DateTime _currentDate;

        private ClaimsPrincipal _principal = null!;

        public GetTasksCommandHandlerTest()
        {
            _smartTaskRepository = new Mock<ISmartTaskRepository>();
            _applicationUserRepository = new Mock<IRepository<ApplicationUser>>();
            _currentDate = DateTime.UtcNow;

            string email = "test@test.com";
            _applicationUserRepository.Setup(x => x.GetAll())
               .ReturnsCollectionOf(new ApplicationUser
               {
                   Email = email,
                   Name = "Test User"
               });
            _smartTaskRepository.Setup(x => x.GetAllBy(It.IsAny<Expression<Func<SmartTask, bool>>>()))
               .ReturnsCollectionOf(Enumerable.Range(1, 5).Select(i =>
               {
                   return new SmartTask
                   {
                       Id = i,
                       Author = email,
                       Title = $"Task {i}",
                       Description = $"Description for Task {i}",
                       DueDate = _currentDate.AddDays(i),
                       CompletionStatus = i % 2 == 0 ? CompletionStatus.Completed : CompletionStatus.Pending,
                       CreatedAt = _currentDate.AddDays(-i)
                   };
               }));

            _principal = new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new(ClaimTypes.Email, email) }));

            _commandHandler = new GetTasksCommandHandler(_smartTaskRepository.Object, _applicationUserRepository.Object);
        }

        [Fact]
        public async Task Handle_ShouldReturnTasksForCurrentUser()
        {
            // Arrange
            var command = new GetTasksCommand
            {
                CurrentPrincipal = _principal,
                StartIndex = 0,
                EndIndex = 2
            };

            // Act
            var response = await _commandHandler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(5, response.TotalCount);
            Assert.Equal(2, response.Tasks.Length);

            var firstTask = response.Tasks[0];
            Assert.Equal(1, firstTask.Id);
            Assert.Equal("Test User", firstTask.AuthorName);
            Assert.Equal("Task 1", firstTask.Title);
            Assert.Equal("Description for Task 1", firstTask.Description);
            Assert.Equal("Incomplete", firstTask.CompletionStatus);
            Assert.Equal(_currentDate.AddDays(1), firstTask.DueDate);
            Assert.Equal(_currentDate.AddDays(-1), firstTask.CreatedAt);

            var secondTask = response.Tasks[1];
            Assert.Equal(2, secondTask.Id);
            Assert.Equal("Test User", secondTask.AuthorName);
            Assert.Equal("Task 2", secondTask.Title);
            Assert.Equal("Description for Task 2", secondTask.Description);
            Assert.Equal("Completed", secondTask.CompletionStatus);
            Assert.Equal(_currentDate.AddDays(2), secondTask.DueDate);
            Assert.Equal(_currentDate.AddDays(-2), secondTask.CreatedAt);
        }
    }
}
