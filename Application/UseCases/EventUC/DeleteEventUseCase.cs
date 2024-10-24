using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;

namespace Application.UseCases.EventUC;

public class DeleteEventUseCase
{
    private readonly IMapper _mapper;
    private readonly IValidator<EventDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteEventUseCase(IMapper mapper, IValidator<EventDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(EventDTO eventDTO)
    {
        var validationResult = await _validator.ValidateAsync(eventDTO);
        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Event is not valid");
        }

        var @event = _mapper.Map<Event>(eventDTO);

        var isDeleted = await _unitOfWork.Events.DeleteAsync(@event);
        if (!isDeleted)
        {
            throw new Exception("Event was not deleted");
        }
    }
}
