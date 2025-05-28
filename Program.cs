using FluentValidation;
using OPTConfigurator.Models;
using OPTConfigurator.Services;
using OPTConfigurator.Services.Interfaces;
using OPTConfigurator.Validations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IOptConfigurationService, OptConfigurationService>();
builder.Services.AddScoped<IValidator<GetOptConfigurationTemplateDTO>, AddOptConfigurationValidator>();
builder.Services.AddScoped<IValidator<UpdateOptConfigurationDTO>, UpdateOptConfigurationRequestValidator>();
builder.Services.AddScoped<IUserInterfaceConfigurationService, UserInterfaceConfigurationService>();

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<ModelBindingErrorHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseCors();
app.MapControllers();
app.Run();