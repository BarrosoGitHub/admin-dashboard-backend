using FluentValidation;
using Microsoft.AspNetCore.HttpOverrides;
using OPTConfigurator.Models;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Validations;
using OPTConfigurator.Helpers;
using OPTConfigurator.Middleware;
using EPSConfigurator.Services;

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

if (Environment.GetEnvironmentVariable("ENABLE_OPT_CONFIGURATION")?.ToLower() == "true")
{
    builder.Services.AddScoped<IOptConfigurationService, OptConfigurationService>();
}
else
{
    builder.Services.AddScoped<IOptConfigurationService, UnavailableService>();
}

if (Environment.GetEnvironmentVariable("ENABLE_EPS_CONFIGURATION")?.ToLower() == "true")
{
    builder.Services.AddScoped<IEpsConfigurationService, EpsConfigurationService>();
}
else
{
    builder.Services.AddScoped<IEpsConfigurationService, UnavailableService>();
}

builder.Services.AddScoped<IUserInterfaceConfigurationService, UserInterfaceConfigurationService>();
builder.Services.AddScoped<IServicesInfoService, ServicesInfoService>();

builder.Services.AddScoped<IValidator<GetOptConfigurationTemplateDTO>, AddOptConfigurationValidator>();
builder.Services.AddScoped<IValidator<UpdateOptConfigurationDTO>, UpdateOptConfigurationRequestValidator>();
builder.Services.AddScoped<IValidator<UserInterfaceConfigurationDTO>, AddUserInterfaceConfigurationValidator>();

string boardType = BoardHelper.GetBoardType();

if (boardType == "Toradex")
{
    builder.Services.AddScoped<INetworkConfigurationService, ToradexNetworkConfigurationService>();
}
else
{
    builder.Services.AddScoped<INetworkConfigurationService, TSNetworkConfigurationService>();
}

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
