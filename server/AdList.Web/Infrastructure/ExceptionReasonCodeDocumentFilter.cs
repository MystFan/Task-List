using AdList.Domain.Exceptions;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AdList.Web.Infrastructure
{
    public class ExceptionReasonCodeDocumentFilter : IDocumentFilter
    {
        public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
            context.SchemaGenerator.GenerateSchema(typeof(ExceptionReasonCode), context.SchemaRepository);
        }
    }
}
