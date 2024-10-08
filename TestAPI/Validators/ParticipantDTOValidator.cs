using FluentValidation;
using TestAPI.DTOs;

namespace TestAPI.Validators;

public class ParticipantDTOValidator : AbstractValidator<ParticipantDTO>
{
    public ParticipantDTOValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
    }
}
