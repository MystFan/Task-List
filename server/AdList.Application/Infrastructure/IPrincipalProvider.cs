using System.Security.Claims;

namespace AdList.Application.Infrastructure;

public interface IPrincipalProvider
{
    ClaimsPrincipal? Current { get; }
}