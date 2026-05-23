using AdList.Application.Abstract.Command;
using AdList.Domain;
using AdList.Infrastructure;
using FluentValidation;

namespace AdList.Application.Commands.CreateSmartTaskCommand
{
    public class CreateSmartTaskCommandValidator : AbstractValidator<CreateSmartTaskCommand>, IFluentValidator
    {
        public CreateSmartTaskCommandValidator(IDateTimeProvider dateTimeProvider)
        {
            RuleFor(c => c.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Constants.SmartTask.TitleMaxLength);

            RuleFor(c => c.Description)
                .MaximumLength(Constants.SmartTask.DescriptionMaxLength);

            RuleFor(c => c.DueDate)
                .Must((command, dueDate) =>
                {
                    return !dueDate.HasValue || dueDate > dateTimeProvider.Now;
                })
                .WithMessage("Due date must not be in the past."); ;
        }
    }
}
