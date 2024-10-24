using Application.DTOs;
using AutoMapper;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.UseCases.ParticipantUC;

public class GetParticipantByIdUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetParticipantByIdUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ParticipantDTO> ExecuteAsync(int participantId)
    {
        var participant = await _unitOfWork.Participants.GetByIdAsync(participantId);
        if (participant is null)
        {
            throw new NotFoundException("Participant not found.");
        }

        var participantDTO = _mapper.Map<ParticipantDTO>(participant);

        return participantDTO;
    }
}
