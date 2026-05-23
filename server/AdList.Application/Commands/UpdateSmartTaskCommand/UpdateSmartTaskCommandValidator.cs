using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Domain;
using AdList.Domain.Entities;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.UpdateSmartTaskCommand
{
    public sealed class UpdateSmartTaskCommandValidator : AbstractValidator<UpdateSmartTaskCommand>, IFluentValidator
    {
        public UpdateSmartTaskCommandValidator(ISmartTaskRepository smartTaskRepository)
        {
            RuleFor(c => c.Title)
                .NotNull()
                .NotEmpty()
                .MaximumLength(Constants.SmartTask.TitleMaxLength);

            RuleFor(c => c.Description)
                .MaximumLength(Constants.SmartTask.DescriptionMaxLength);

            RuleFor(c => c.CompletionStatus)
                .NotNull()
                .MustAsync(async (command, completionStatus, cancellationToken) =>
                {
                    SmartTask? dbSmartTask = await smartTaskRepository.GetAll().FirstOrDefaultAsync(t => t.Id == command.Id, cancellationToken);
                    if (dbSmartTask == null)
                    {
                        return true;
                    }

                    return (dbSmartTask.CompletionStatus == CompletionStatus.Pending && completionStatus is CompletionStatus.Pending or CompletionStatus.Completed) ||
                        (dbSmartTask.CompletionStatus == CompletionStatus.Completed && completionStatus == CompletionStatus.Completed);
                })
                .WithMessage("Invalid completion status transition.");
        }
    }
}
