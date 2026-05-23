using System.Security.Claims;

namespace AdList.Application.Infrastructure;

public interface ICurrentPrincipalRequest
{
    public ClaimsPrincipal? CurrentPrincipal { get; set; }
}