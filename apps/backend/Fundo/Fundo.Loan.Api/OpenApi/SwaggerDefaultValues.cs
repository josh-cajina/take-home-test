using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Fundo.Loan.Api.OpenApi;

public class SwaggerDefaultValues : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        Microsoft.AspNetCore.Mvc.ApiExplorer.ApiDescription apiDescription = context.ApiDescription;

        operation.Deprecated = apiDescription.IsDeprecated();

        if (operation.Parameters == null)
        {
            return;
        }

        foreach (OpenApiParameter? parameter in operation.Parameters)
        {
            ApiParameterDescription description = apiDescription.ParameterDescriptions.First(p => p.Name == parameter.Name);
            parameter.Description ??= description.ModelMetadata?.Description;
            if (parameter.Schema.Default == null && description.DefaultValue != null)
            {
                parameter.Schema.Default = new Microsoft.OpenApi.Any.OpenApiString(description.DefaultValue.ToString());
            }
            parameter.Required |= description.IsRequired;
        }
    }
}
