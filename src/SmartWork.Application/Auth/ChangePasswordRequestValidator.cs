using FluentValidation;

namespace SmartWork.Application.Auth;

public sealed class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword).NotEmpty();
        RuleFor(x => x.NewPassword).NotEmpty().MinimumLength(8).NotEqual(x => x.CurrentPassword).WithMessage("新密码不能与当前密码相同。");
    }
}
