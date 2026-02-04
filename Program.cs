using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using OPTConfigurator.Models;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Helpers;
using OPTConfigurator.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders =
        ForwardedHeaders.XForwardedFor |
        ForwardedHeaders.XForwardedProto |
        ForwardedHeaders.XForwardedHost;

    options.KnownNetworks.Clear();
    options.KnownProxies.Clear();
});


builder.Services.AddScoped<IServicesInfoService, ServicesInfoService>();


string boardType = BoardHelper.GetBoardType();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseForwardedHeaders();
app.UseWebSockets();
app.UseMiddleware<RebootingStateMiddleware>();
app.UseMiddleware<ModelBindingErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();
