using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

public class ModelBindingErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;

    public ModelBindingErrorHandlingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Capture the response
        var originalBody = context.Response.Body;
        using var newBody = new MemoryStream();
        context.Response.Body = newBody;

        await _next(context);

        // If model binding failed (400), try to parse and customize the error
        if (context.Response.StatusCode == 400 && context.Response.ContentType?.Contains("application/problem+json") == true)
        {
            newBody.Seek(0, SeekOrigin.Begin);
            var problemJson = await new StreamReader(newBody).ReadToEndAsync();

            var problem = JsonSerializer.Deserialize<ValidationProblemDetails>(problemJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var customErrors = new List<string>();

            if (problem?.Errors != null)
            {
                foreach (var kvp in problem.Errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        if (error.Contains("could not be converted"))
                        {
                            customErrors.Add($"Field '{kvp.Key.Replace("$.","")}' has an invalid type or value.");
                        }
                        else if (error.Contains("required"))
                        {
                            customErrors.Add($"Field '{kvp.Key.Replace("$.","")}' is missing.");
                        }
                        else
                        {
                            customErrors.Add(error);
                        }
                    }
                }
            }

            var customResponse = new
            {
                status = 400,
                errors = customErrors
            };

            context.Response.Body = originalBody;
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = 400;
            await context.Response.WriteAsync(JsonSerializer.Serialize(customResponse));
            return;
        }

        newBody.Seek(0, SeekOrigin.Begin);
        await newBody.CopyToAsync(originalBody);
    }
}