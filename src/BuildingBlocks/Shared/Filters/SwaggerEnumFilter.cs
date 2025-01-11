using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Shared.Filters;

public class SwaggerEnumFilter : ISchemaFilter
{
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type.IsEnum)
        {
            schema.Enum.Clear();
            Enum.GetNames(context.Type)
                .ToList()
                .ForEach(name => schema.Enum.Add(new OpenApiString($"{Convert.ToInt64(Enum.Parse(context.Type, name))}")));
        }
    }
}