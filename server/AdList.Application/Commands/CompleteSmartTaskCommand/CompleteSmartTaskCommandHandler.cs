using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Extensions;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.CompleteSmartTaskCommand
{
    public sealed class CompleteSmartTaskCommandHandler(ISmartTaskRepository smartTaskRepository) : ICommandHandler<CompleteSmartTaskCommand, EmptyResponse>
    {
        public async Task<EmptyResponse> Handle(CompleteSmartTaskCommand request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            SmartTask? task = await smartTaskRepository.GetAll().FirstOrDefaultAsync(t => t.Id == request.Id && t.Author == email, cancellationToken);
            if (task == null)
            {
                throw new DomainException(ExceptionReasonCode.TaskNotFound, "Task not found.");
            }

            if (task.CompletionStatus == CompletionStatus.Completed) 
            {
                throw new DomainException(ExceptionReasonCode.TaskAlreadyCompleted, "Invalid completion status transition.");
            }

            task.CompletionStatus = CompletionStatus.Completed;

            await smartTaskRepository.UpdateAsync(task, cancellationToken);

            return EmptyResponse.Instance;
        }
    }
}
