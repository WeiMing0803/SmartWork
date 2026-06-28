using FluentValidation;

namespace SmartWork.Application.Auth;

public sealed class RegisterRequestValidator : AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.UserName).NotEmpty().MinimumLength(2).MaximumLength(64);
        RuleFor(x => x.Password).NotEmpty().MinimumLength(8);
    }
}
