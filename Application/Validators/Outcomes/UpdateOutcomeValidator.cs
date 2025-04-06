using Application.DTOs.Outcomes;
using FluentValidation;

namespace Application.Validators.Outcomes;

public class UpdateOutcomeValidator : AbstractValidator<UpdateOutcomeDto>
{
    public UpdateOutcomeValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Description)
            .MaximumLength(250).WithMessage("Description must not exceed 250 characters");
    }
}
