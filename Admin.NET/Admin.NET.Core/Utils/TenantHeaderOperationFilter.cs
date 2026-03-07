using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Admin.NET.Core;

/// <summary>
/// Tenant header parameter filter
/// </summary>
public class TenantHeaderOperationFilter : IOperationFilter
{
    /// <summary>
    /// Apply tenant header parameters filter
    /// </summary>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= [];

        operation.Parameters.Add(new OpenApiParameter
        {
            Name = ClaimConst.TenantId,
            In = ParameterLocation.Header,
            Schema = new OpenApiSchema { Type = JsonSchemaType.String },
            Required = false,
            AllowEmptyValue = true,
            Description = "Tenant ID (leave blank for default tenant)"
        });
    }
}