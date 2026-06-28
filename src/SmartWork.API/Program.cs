using FluentValidation;
using Hangfire;
using Scalar.AspNetCore;
using Serilog;
using SmartWork.API.Services;
using SmartWork.Application;
using SmartWork.Application.Common.Interfaces;
using SmartWork.Infrastructure;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<SmartWork.Application.Auth.RegisterRequestValidator>();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Title = "SmartWork API");
    app.UseHangfireDashboard("/jobs");
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", app = "SmartWork.API" })).WithTags("System");
app.Run();
