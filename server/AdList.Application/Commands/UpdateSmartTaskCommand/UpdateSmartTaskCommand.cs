using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Infrastructure;
using AdList.Domain.Entities;
using System.Security.Claims;

namespace AdList.Application.Commands.UpdateSmartTaskCommand
{
    public record UpdateSmartTaskCommand : ICommand<EmptyResponse>, ICurrentPrincipalRequest
    {
        public long Id { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public CompletionStatus CompletionStatus { get; set; }

        public ClaimsPrincipal? CurrentPrincipal { get; set; }
    }
}
