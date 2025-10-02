using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OPTConfigurator.Models;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Validations;
using System.Text;
using OPTConfigurator.Helpers;
using EPSConfigurator.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("auth.json", optional: false, reloadOnChange: true);

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
builder.Services.AddScoped<IValidator<GetOptConfigurationTemplateDTO>, AddOptConfigurationValidator>();
builder.Services.AddScoped<IValidator<UpdateOptConfigurationDTO>, UpdateOptConfigurationRequestValidator>();
builder.Services.AddScoped<IValidator<UserInterfaceConfigurationDTO>, AddUserInterfaceConfigurationValidator>();
builder.Services.AddScoped<IServicesInfoService, ServicesInfoService>();

string boardType = BoardHelper.GetBoardType();


if (boardType == "Toradex")
{
    builder.Services.AddScoped<INetworkConfigurationService, ToradexNetworkConfigurationService>();
    Console.WriteLine("Using Toradex network configuration service.");
}
else if (boardType == "TS7970")
{
    builder.Services.AddScoped<INetworkConfigurationService, TSNetworkConfigurationService>();
    Console.WriteLine("Using TS7970 network configuration service.");
}
else
{
    // Console.WriteLine("Unknown board type. Exiting application.");
    // Environment.Exit(1);
    builder.Services.AddScoped<INetworkConfigurationService, TSNetworkConfigurationService>();

}

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    });

builder.Services.AddOpenApi();

var jwtKey = builder.Configuration["Jwt:Key"];
if (!string.IsNullOrEmpty(jwtKey))
{
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.FromSeconds(30),
        };
    });
}

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });

    // options.AddDefaultPolicy(policy =>
    // {
    //     policy.WithOrigins("http://172.16.55.152:8081")
    //           .AllowAnyHeader()
    //           .AllowAnyMethod()
    //           .AllowCredentials();
    // });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseWebSockets();
app.UseMiddleware<ModelBindingErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();