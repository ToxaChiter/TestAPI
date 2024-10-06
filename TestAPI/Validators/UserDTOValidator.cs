using FluentValidation;
using TestAPI.DTOs;

namespace TestAPI.Validators;

public class UserDTOValidator : AbstractValidator<UserDTO>
{
    public UserDTOValidator()
    {
        RuleFor(user => user.Login)
            .NotEmpty().WithMessage("Login is required.")
            .Length(2, 50).WithMessage("Login must be between 2 and 50 characters.");

        RuleFor(user => user.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Length(8, 24).WithMessage("Password must be between 8 and 24 characters.");
    }
}