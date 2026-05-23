using System.Security.Claims;
using AdList.Application.Infrastructure;

namespace AdList.Web.Infrastructure;

internal sealed class PrincipalProvider(IHttpContextAccessor httpContextAccessor) : IPrincipalProvider
{
    public ClaimsPrincipal? Current
    {
        get => httpContextAccessor.HttpContext?.User;
    }
}