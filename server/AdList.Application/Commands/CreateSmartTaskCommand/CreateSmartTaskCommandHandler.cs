
using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Extensions;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.CreateSmartTaskCommand
{
    public sealed class CreateSmartTaskCommandHandler(ISmartTaskRepository smartTaskRepository, IRepository<ApplicationUser> applicationUserRepository) : ICommandHandler<CreateSmartTaskCommand, EmptyResponse>
    {
        public async Task<EmptyResponse> Handle(CreateSmartTaskCommand request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            ApplicationUser? user = await applicationUserRepository.GetAll().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                throw new DomainException(ExceptionReasonCode.UserNotFound, "User not found.");
            }

            var task = new SmartTask
            {
                Author = user.Email,
                Title = request.Title,
                Description = request.Description,
                DueDate = request.DueDate,
                CompletionStatus = CompletionStatus.Pending
            };

            await smartTaskRepository.CreateAsync(task, cancellationToken);

            return EmptyResponse.Instance;
        }
    }
}
