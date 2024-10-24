using Application.DTOs;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.ParticipantUC;

public class GetAllParticipantsUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllParticipantsUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<ParticipantDTO>> ExecuteAsync()
    {
        var participants = await _unitOfWork.Participants.GetAllAsync();
        var participantsDTO = _mapper.ProjectTo<ParticipantDTO>(participants.AsQueryable());

        return participantsDTO;
    }
}
