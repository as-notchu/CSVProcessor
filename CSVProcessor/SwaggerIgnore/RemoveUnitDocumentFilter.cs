using CSVProcessor.Models;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace CSVProcessor.SwaggerIgnore;

public class RemoveUnitDocumentFilter : IDocumentFilter
{
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        context.SchemaRepository.Schemas.Remove("Unit");
    }
}