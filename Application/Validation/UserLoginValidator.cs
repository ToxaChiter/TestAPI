using Core.Entities;
using FluentValidation;

namespace Application.Validation;

public class UserLoginValidator : AbstractValidator<UserLogin>
{
    public UserLoginValidator()
    {
        RuleFor(user => user.Email)
            .EmailAddress();

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 24).WithMessage("Password must be between 8 and 24 characters.");
    }
}