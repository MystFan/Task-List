using AdList.Application.Abstract;
using AdList.Application.Abstract.Command;
using AdList.Application.Extensions;
using AdList.Application.Infrastructure;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Commands.GetTasksCommand
{
    public sealed class GetTasksCommandHandler(IRepository<SmartTask> smartTaskRepository, IRepository<ApplicationUser> applicationUserRepository) : ICommandHandler<GetTasksCommand, GetTasksCommandResponse>
    {
        public async Task<GetTasksCommandResponse> Handle(GetTasksCommand request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            var user = await applicationUserRepository.GetAll().FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
            if (user == null)
            {
                throw new DomainException(ExceptionReasonCode.UserNotFound, "User not found.");
            }

            var tasksQuery = smartTaskRepository.GetAllBy(t => t.Author == email)
                .OrderByDescending(t => t.CreatedAt)
                .Select(t => new GetTaskCommandResponse
                {
                    Id = t.Id,
                    AuthorName = user.Name!,
                    Title = t.Title,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    CompletionStatus = t.CompletionStatus == CompletionStatus.Completed ? "Completed" : "Incomplete",
                    CreatedAt = t.CreatedAt
                });

            tasksQuery = QueryOrderer.ApplyOrdering(tasksQuery, request.Sorts);

            int skip = request.StartIndex ?? 0;
            int take = request.EndIndex ?? 50 - skip;

            return new GetTasksCommandResponse
            {
                Tasks = await tasksQuery.Skip(skip).Take(take).ToArrayAsync(cancellationToken),
                TotalCount = await tasksQuery.CountAsync()
            };
        }
    }
}
