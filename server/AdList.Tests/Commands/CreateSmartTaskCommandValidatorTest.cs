using AdList.Application.Commands.CreateSmartTaskCommand;
using AdList.Domain;
using AdList.Infrastructure;
using System.Security.Claims;

namespace AdList.Tests.Commands
{
    public class CreateSmartTaskCommandValidatorTest
    {
        private readonly CreateSmartTaskCommandValidator _validator;
        private readonly ClaimsPrincipal _testPrincipal;
        private DateTime _now = new DateTime(2024, 6, 1, 10, 10, 10, 10);

        public CreateSmartTaskCommandValidatorTest()
        {
            var dateTimeProvider = new Mock<IDateTimeProvider>();
            dateTimeProvider.Setup(x => x.Now).Returns(_now);
            _validator = new CreateSmartTaskCommandValidator(dateTimeProvider.Object);

            // Setup test principal
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, "test@example.com")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            _testPrincipal = new ClaimsPrincipal(identity);
        }

        #region Title Validation Tests

        [Fact]
        public void Validate_WithValidTitle_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithNullTitle_IsInvalid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = null!,
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_WithEmptyTitle_IsInvalid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = string.Empty,
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_WithTitleExceedingMaxLength_IsInvalid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = new string('A', Constants.SmartTask.TitleMaxLength + 1),
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        [Fact]
        public void Validate_WithTitleAtMaxLength_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = new string('A', Constants.SmartTask.TitleMaxLength),
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithWhitespaceTitle_IsInvalid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "  ",
                Description = "Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
        }

        #endregion

        #region Description Validation Tests

        [Fact]
        public void Validate_WithValidDescription_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithNullDescription_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = null,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithEmptyDescription_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = string.Empty,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithDescriptionExceedingMaxLength_IsInvalid()
        {
            // Arrange
            var longDescription = new string('A', Constants.SmartTask.DescriptionMaxLength + 1);
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = longDescription,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "Description");
        }

        [Fact]
        public void Validate_WithDescriptionAtMaxLength_IsValid()
        {
            // Arrange
            var maxLengthDescription = new string('A', Constants.SmartTask.DescriptionMaxLength);
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = maxLengthDescription,
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        #endregion

        #region DueDate Validation Tests

        [Fact]
        public void Validate_WithFutureDueDate_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = DateTime.UtcNow.AddDays(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithNullDueDate_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = null
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public void Validate_WithPastDueDate_IsInvalid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = _now.AddSeconds(-1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
            Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
            Assert.Contains(result.Errors, e => e.ErrorMessage == "Due date must not be in the past.");
        }

        [Fact]
        public void Validate_WithDueDateInPast_HasCorrectErrorMessage()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = _now.AddSeconds(-1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            var dueDateError = result.Errors.FirstOrDefault(e => e.PropertyName == "DueDate");
            Assert.NotNull(dueDateError);
            Assert.Equal("Due date must not be in the past.", dueDateError.ErrorMessage);
        }

        [Fact]
        public void Validate_WithDueDateOneSecondInFuture_IsValid()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = "Valid Title",
                Description = "Valid Description",
                DueDate = _now.AddSeconds(1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        #endregion

        #region Multiple Validation Errors Tests

        [Fact]
        public void Validate_WithMultipleErrors_ReturnsAllErrors()
        {
            // Arrange
            var longTitle = new string('A', Constants.SmartTask.TitleMaxLength + 1);
            var longDescription = new string('B', Constants.SmartTask.DescriptionMaxLength + 1);
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = longTitle,
                Description = longDescription,
                DueDate = _now.AddSeconds(-1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Equal(3, result.Errors.Count);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
            Assert.Contains(result.Errors, e => e.PropertyName == "Description");
            Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
        }

        [Fact]
        public void Validate_WithNullTitleAndPastDueDate_ReturnsBothErrors()
        {
            // Arrange
            var command = new CreateSmartTaskCommand
            {
                CurrentPrincipal = _testPrincipal,
                Title = null!,
                Description = "Valid Description",
                DueDate = _now.AddSeconds(-1)
            };

            // Act
            var result = _validator.Validate(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.True(result.Errors.Count >= 2);
            Assert.Contains(result.Errors, e => e.PropertyName == "Title");
            Assert.Contains(result.Errors, e => e.PropertyName == "DueDate");
        }

        #endregion
    }
}
