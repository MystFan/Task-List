using System.Security.Claims;

namespace AdList.Application.Extensions
{
    internal static class CurrentPrincipalExtensions
    {
        public static string GetEmailEnsured(this ClaimsPrincipal? principal)
        {
            Claim? email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email);

            if (email == null)
            {
                throw new InvalidOperationException("User email is not found");
            }

            return email.Value;
        }
    }
}
