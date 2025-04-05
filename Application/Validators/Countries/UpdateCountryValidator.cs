using Application.DTOs.Countries;
using FluentValidation;

namespace Application.Validators.Countries;

public class UpdateCountryValidator : AbstractValidator<UpdateCountryDto>
{
    public UpdateCountryValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(100).WithMessage("Name must not exceed 100 characters");

        RuleFor(x => x.Code)
            .NotEmpty().WithMessage("Code is required")
            .Length(2).WithMessage("Code must be exactly 2 characters");
    }
}
