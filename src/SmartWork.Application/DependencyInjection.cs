using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using SmartWork.Application.Auth;

namespace SmartWork.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        // 注册 FluentValidation 验证器：扫描 RegisterRequestValidator 所在程序集（SmartWork.Application），
        // 把该程序集内所有 AbstractValidator<T> 实现一次性注册到 DI。
        // 因此只需写一个验证器当"程序集定位锚"，其余验证器（Login/Refresh/ChangePassword 等）
        // 只要和它在同一程序集就会自动被注册，无需逐个 Add。
        services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
        return services;
    }
}
