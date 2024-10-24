using Core.Entities;
using FluentValidation;

namespace Application.Validation;

public class UserRegValidator : AbstractValidator<UserRegistration>
{
    public UserRegValidator()
    {
        RuleFor(user => user.DateOfBirth.Year)
            .LessThanOrEqualTo(DateTime.UtcNow.AddYears(-6).Year)
            .GreaterThan(DateTime.UtcNow.AddYears(-120).Year);

        RuleFor(user => user.Email)
            .EmailAddress();

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 24).WithMessage("Password must be between 8 and 24 characters.");
    }
}
