using FluentValidation;
using TestAPI.DTOs;

namespace TestAPI.Validators;

public class EventDTOValidator : AbstractValidator<EventDTO>
{
    public EventDTOValidator()
    {
        
    }
}
