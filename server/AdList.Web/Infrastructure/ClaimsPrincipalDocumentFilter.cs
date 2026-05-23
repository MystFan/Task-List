using System.Security.Claims;
using System.Security.Principal;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdList.Web.Infrastructure.Swagger;

public class ClaimsPrincipalDocumentFilter : IDocumentFilter
{
    private static readonly string[] ExcludedKeys = [nameof(ClaimsPrincipal), nameof(ClaimsIdentity), nameof(Claim), nameof(IIdentity)];

    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        foreach (string key in swaggerDoc.Components.Schemas.Keys)
        {
            if (ExcludedKeys.Contains(key))
            {
                swaggerDoc.Components.Schemas.Remove(key);
            }
        }
    }
}