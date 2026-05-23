using AdList.Application.Infrastructure;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdList.Web.Infrastructure.Swagger;

public class CurrentPrincipalSchemaFilter : ISchemaFilter
{
    private const string ExcludedKey = nameof(ICurrentPrincipalRequest.CurrentPrincipal);

    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        string? propertyToRemove = schema.Properties.Keys.SingleOrDefault(x => string.Equals(x, ExcludedKey, StringComparison.OrdinalIgnoreCase));

        if (propertyToRemove != null)
        {
            schema.Properties.Remove(propertyToRemove);
        }
    }
}