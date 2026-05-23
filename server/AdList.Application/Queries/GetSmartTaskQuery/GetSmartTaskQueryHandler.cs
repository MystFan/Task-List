using AdList.Application.Abstract;
using AdList.Application.Abstract.Query;
using AdList.Application.Extensions;
using AdList.Domain.Entities;
using AdList.Domain.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace AdList.Application.Queries.GetSmartTaskQuery
{
    public sealed class GetSmartTaskQueryHandler(ISmartTaskRepository smartTaskRepository) : IQueryHandler<GetSmartTaskQuery, GetSmartTaskQueryResponse>
    {
        public async Task<GetSmartTaskQueryResponse> Handle(GetSmartTaskQuery request, CancellationToken cancellationToken)
        {
            string email = request.CurrentPrincipal.GetEmailEnsured();

            SmartTask? task = await smartTaskRepository.GetAll().FirstOrDefaultAsync(t => t.Id == request.Id && t.Author == email, cancellationToken);
            if (task == null)
            {
                throw new DomainException(ExceptionReasonCode.TaskNotFound, "Task not found.");
            }

            return new GetSmartTaskQueryResponse
            {
                Id = task.Id,
                Title = task.Title,
                Description = task.Description,
                DueDate = task.DueDate,
                CompletionStatus = task.CompletionStatus == CompletionStatus.Completed ? "Completed" : "Incomplete",
                AuthorName = task.Author,
                CreatedAt = task.CreatedAt
            };
        }
    }
}
