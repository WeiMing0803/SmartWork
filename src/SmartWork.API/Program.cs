using FluentValidation;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Scalar.AspNetCore;
using Serilog;
using SmartWork.API.Middleware;
using SmartWork.API.Services;
using SmartWork.Application;
using SmartWork.Application.Auth;
using SmartWork.Application.Common.Interfaces;
using SmartWork.Infrastructure;
using SmartWork.Infrastructure.Identity;
using SmartWork.Infrastructure.Persistence;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((context, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(context.Configuration));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();
builder.Services.AddOpenApi();

WebApplication app = builder.Build();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseSerilogRequestLogging();
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options => options.Title = "SmartWork API");
    app.MapGet("/", () => Results.Redirect("/scalar"));
    app.UseHangfireDashboard("/jobs");

    // 开发环境初始化默认角色
    using (IServiceScope scope = app.Services.CreateScope())
    {
        RoleManager<ApplicationRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
        await DbInitializer.InitializeAsync(roleManager, scope.ServiceProvider.GetService<ILoggerFactory>()?.CreateLogger("DbInitializer"));
    }
}
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "Healthy", app = "SmartWork.API" })).WithTags("System");
app.Run();
