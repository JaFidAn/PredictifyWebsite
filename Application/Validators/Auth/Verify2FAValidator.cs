using Application.DTOs.Auth;
using FluentValidation;

namespace Application.Validators.Auth;

public class Verify2FAValidator : AbstractValidator<Verify2FADto>
{
    public Verify2FAValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email is not valid");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Verification code is required")
            .Length(6).WithMessage("Verification code must be 6 digits");
    }
}
