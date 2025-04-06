using Application.DTOs.Competitions;
using FluentValidation;

namespace Application.Validators.Competitions;

public class UpdateCompetitionValidator : AbstractValidator<UpdateCompetitionDto>
{
    public UpdateCompetitionValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(150).WithMessage("Name must not exceed 150 characters");

        RuleFor(x => x.IsInternational)
            .NotNull().WithMessage("IsInternational is required");
    }
}
