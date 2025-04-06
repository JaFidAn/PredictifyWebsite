using Application.DTOs.Competitions;
using FluentValidation;

namespace Application.Validators.Competitions;

public class CreateCompetitionValidator : AbstractValidator<CreateCompetitionDto>
{
    public CreateCompetitionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.IsInternational)
            .NotNull().WithMessage("IsInternational is required");
    }
}
