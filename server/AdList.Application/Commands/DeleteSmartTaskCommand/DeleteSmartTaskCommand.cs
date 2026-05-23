using AdList.Application.Abstract.Command;
using AdList.Application.Abstract.Implementation;
using AdList.Application.Infrastructure;
using System.Security.Claims;

namespace AdList.Application.Commands.DeleteSmartTaskCommand
{
    public record DeleteSmartTaskCommand : ICommand<EmptyResponse>, ICurrentPrincipalRequest
    {
        public long Id { get; set; }

        public ClaimsPrincipal? CurrentPrincipal { get; set; }
    }
}
