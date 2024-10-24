using Application.DTOs;
using AutoMapper;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.UseCases.ParticipantUC;

public class GetEventParticipantsUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventParticipantsUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ParticipantDTO>> ExecuteAsync(int eventId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        var participantsDTO = _mapper.ProjectTo<ParticipantDTO>(eventEntity.Participants.AsQueryable());

        return participantsDTO; 
    }
}
