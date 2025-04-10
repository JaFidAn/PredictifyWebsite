using Application.DTOs.Outcomes;
using FluentValidation;

namespace Application.Validators.Outcomes;

public class CreateOutcomeValidator : AbstractValidator<CreateOutcomeDto>
{
    public CreateOutcomeValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .MaximumLength(50).WithMessage("Code must not exceed 50 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters");
    }
}
