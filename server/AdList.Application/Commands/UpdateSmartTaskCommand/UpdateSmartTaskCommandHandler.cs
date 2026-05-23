using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Extensions;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.UpdateSmartTaskCommand
{
    internal sealed class UpdateSmartTaskCommandHandler(ISmartTaskRepository smartTaskRepository) : ICommandHandler<UpdateSmartTaskCommand, EmptyResponse>
    {
        public async Task<EmptyResponse> Handle(UpdateSmartTaskCommand request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            SmartTask? task = await smartTaskRepository.GetAll().FirstOrDefaultAsync(t => t.Id == request.Id && t.Author == email, cancellationToken);
            if (task == null)
            {
                throw new DomainException(ExceptionReasonCode.TaskNotFound, "Task not found.");
            }

            task.Title = request.Title;
            task.Description = request.Description;
            task.DueDate = request.DueDate;
            task.CompletionStatus = request.CompletionStatus;

            await smartTaskRepository.UpdateAsync(task, cancellationToken);

            return EmptyResponse.Instance;
        }
    }
}
