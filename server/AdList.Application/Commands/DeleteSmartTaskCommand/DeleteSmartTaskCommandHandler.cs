using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Extensions;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.DeleteSmartTaskCommand
{
    public sealed class DeleteSmartTaskCommandHandler(ISmartTaskRepository smartTaskRepository) : ICommandHandler<DeleteSmartTaskCommand, EmptyResponse>
    {
        public async Task<EmptyResponse> Handle(DeleteSmartTaskCommand request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            SmartTask? task = await smartTaskRepository.GetAll().FirstOrDefaultAsync(t => t.Id == request.Id && t.Author == email, cancellationToken);
            if (task == null)
            {
                throw new DomainException(ExceptionReasonCode.TaskNotFound, "Task not found.");
            }

            await smartTaskRepository.DeleteAsync(task, cancellationToken);

            return EmptyResponse.Instance;
        }
    }
}
