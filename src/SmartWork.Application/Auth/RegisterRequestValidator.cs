using FluentValidation;

namespace SmartWork.Application.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        // 每个校验器后紧跟 .WithMessage(...) 自定义失败消息，互不覆盖。
        // 不加 WithMessage 时使用 FluentValidation 默认消息（英文，或配置了中文资源后为中文）。
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("邮箱不能为空。")
            .EmailAddress().WithMessage("邮箱格式不正确。");

        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage("用户名不能为空。")
            .MinimumLength(2).WithMessage("用户名至少需要 2 个字符。")
            .MaximumLength(64).WithMessage("用户名不能超过 64 个字符。");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("密码不能为空。")
            .MinimumLength(8).WithMessage("密码至少需要 8 个字符。");
    }
}
