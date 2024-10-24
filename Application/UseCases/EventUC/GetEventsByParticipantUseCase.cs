using Application.DTOs;
using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;

namespace Application.UseCases.EventUC;

public class GetEventsByParticipantUseCase
{
    private readonly IMapper _mapper;
    private readonly IValidator<ParticipantDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventsByParticipantUseCase(IMapper mapper, IValidator<ParticipantDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<EventDTO>> ExecuteAsync(int participantId)
    {
        var events = await _unitOfWork.Events.GetAllByParticipantAsync(participantId);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());

        return eventsDTO;
    }
}
