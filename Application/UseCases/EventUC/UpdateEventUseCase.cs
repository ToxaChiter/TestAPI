using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;

namespace Application.UseCases.EventUC;

public class UpdateEventUseCase
{
    private readonly IMapper _mapper;
    private readonly IValidator<EventDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateEventUseCase(IMapper mapper, IValidator<EventDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<Event> ExecuteAsync(EventDTO eventDTO)
    {
        var validationResult = await _validator.ValidateAsync(eventDTO);
        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Event is not valid");
        }

        var @event = _mapper.Map<Event>(eventDTO);

        var updated = await _unitOfWork.Events.UpdateAsync(@event);
        if (updated is null)
        {
            throw new Exception("Event was not updated");
        }

        return updated;
    }
}
