using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VetCRM.Api.Swagger;

public sealed class SwaggerExamplesOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        string? path = context.ApiDescription.RelativePath;
        string? method = context.ApiDescription.HttpMethod;

        if (path is null)
            return;

        if (path.Equals("api/auth/register", StringComparison.OrdinalIgnoreCase) && method == "POST")
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["email"] = new OpenApiString("receptionist@vetcrm.local"),
                ["password"] = new OpenApiString("Pass123!"),
                ["role"] = new OpenApiInteger(2)
            });
            return;
        }

        if (path.Equals("api/auth/login", StringComparison.OrdinalIgnoreCase) && method == "POST")
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["email"] = new OpenApiString("admin@vetcrm.local"),
                ["password"] = new OpenApiString("Admin123!")
            });
            return;
        }

        if (path.Equals("api/clients", StringComparison.OrdinalIgnoreCase) && method == "POST")
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["fullName"] = new OpenApiString("Иван Петров"),
                ["phone"] = new OpenApiString("+79001234567"),
                ["email"] = new OpenApiString("ivan@example.com"),
                ["address"] = new OpenApiNull(),
                ["notes"] = new OpenApiNull()
            });
            return;
        }

        if (path.Equals("api/pets", StringComparison.OrdinalIgnoreCase) && method == "POST")
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["name"] = new OpenApiString("Барсик"),
                ["species"] = new OpenApiString("Кот"),
                ["birthDate"] = new OpenApiString("2022-05-15T00:00:00Z"),
                ["clientId"] = new OpenApiString("00000000-0000-0000-0000-000000000001")
            });
            return;
        }

        if (path.StartsWith("api/appointments", StringComparison.OrdinalIgnoreCase) && method == "POST" && !path.Contains("complete"))
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["petId"] = new OpenApiString("00000000-0000-0000-0000-000000000001"),
                ["clientId"] = new OpenApiString("00000000-0000-0000-0000-000000000001"),
                ["veterinarianUserId"] = new OpenApiString("00000000-0000-0000-0000-000000000002"),
                ["startsAt"] = new OpenApiString("2025-02-15T10:00:00Z"),
                ["endsAt"] = new OpenApiString("2025-02-15T10:30:00Z"),
                ["reason"] = new OpenApiString("Осмотр")
            });
            return;
        }

        if (path.Contains("complete", StringComparison.OrdinalIgnoreCase) && method == "PUT")
        {
            SetRequestBodyExample(operation, new OpenApiObject
            {
                ["complaint"] = new OpenApiString("Вялость"),
                ["diagnosis"] = new OpenApiString("ОРЗ"),
                ["treatmentPlan"] = new OpenApiString("Покой, обильное питьё"),
                ["prescription"] = new OpenApiString("Витамины"),
                ["attachments"] = new OpenApiNull()
            });
            return;
        }

        if (path.StartsWith("api/reports/appointments", StringComparison.OrdinalIgnoreCase) && method == "GET")
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            if (operation.Parameters.FirstOrDefault(p => p.Name == "from") is { } fromParam)
                fromParam.Example = new OpenApiString("2025-01-01");
            if (operation.Parameters.FirstOrDefault(p => p.Name == "to") is { } toParam)
                toParam.Example = new OpenApiString("2025-01-31");
            if (operation.Parameters.FirstOrDefault(p => p.Name == "page") is { } pageParam)
                pageParam.Example = new OpenApiInteger(1);
            if (operation.Parameters.FirstOrDefault(p => p.Name == "pageSize") is { } sizeParam)
                sizeParam.Example = new OpenApiInteger(20);
            return;
        }

        if (path.StartsWith("api/reports/overdue-vaccinations", StringComparison.OrdinalIgnoreCase) && method == "GET")
        {
            operation.Parameters ??= new List<OpenApiParameter>();
            if (operation.Parameters.FirstOrDefault(p => p.Name == "page") is { } pageParam)
                pageParam.Example = new OpenApiInteger(1);
            if (operation.Parameters.FirstOrDefault(p => p.Name == "pageSize") is { } sizeParam)
                sizeParam.Example = new OpenApiInteger(20);
        }
    }

    private static void SetRequestBodyExample(OpenApiOperation operation, OpenApiObject example)
    {
        if (operation.RequestBody?.Content?.TryGetValue("application/json", out var mediaType) != true || mediaType is null)
            return;
        mediaType.Example = example;
    }
}
