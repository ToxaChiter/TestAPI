using Application.DTOs;
using FluentValidation;

namespace Application.Validation;

public class EventDTOValidator : AbstractValidator<EventDTO>
{
    public EventDTOValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Description).MaximumLength(1000);
        RuleFor(x => x.MaxParticipants).GreaterThan(-1);
    }
}
