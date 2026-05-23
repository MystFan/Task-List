using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Infrastructure;
using System.Security.Claims;

namespace AdList.Application.Commands.CreateSmartTaskCommand
{
    public record CreateSmartTaskCommand : ICommand<EmptyResponse>, ICurrentPrincipalRequest
    {
        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime? DueDate { get; set; }

        public ClaimsPrincipal? CurrentPrincipal { get; set; }
    }
}
