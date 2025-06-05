using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using OPTConfigurator.Models;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Validations;
using System.Text;
using OPTConfigurator.Helpers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IOptConfigurationService, OptConfigurationService>();
builder.Services.AddScoped<IUserInterfaceConfigurationService, UserInterfaceConfigurationService>();
builder.Services.AddScoped<IValidator<GetOptConfigurationTemplateDTO>, AddOptConfigurationValidator>();
builder.Services.AddScoped<IValidator<UpdateOptConfigurationDTO>, UpdateOptConfigurationRequestValidator>();
builder.Services.AddScoped<IValidator<UserInterfaceConfigurationDTO>, AddUserInterfaceConfigurationValidator>();
builder.Services.AddScoped<OPTConfigurator.Services.Interfaces.IServicesInfoService, OPTConfigurator.Services.ServicesInfoService>();

string boardType = BoardHelper.GetBoardType();

if (boardType == "Toradex")
{
    builder.Services.AddScoped<INetworkConfigurationService, ToradexNetworkConfigurationService>();
}
else if (boardType == "TS7970")
{
    builder.Services.AddScoped<INetworkConfigurationService, TSNetworkConfigurationService>();
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

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ModelBindingErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();