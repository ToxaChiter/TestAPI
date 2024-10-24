using Application.DTOs;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.EventUC;

public class GetEventsByDateUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventsByDateUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<EventDTO>> ExecuteAsync(DateOnly date)
    {
        var events = await _unitOfWork.Events.GetAllByDateAsync(date);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());

        return eventsDTO;
    }
}
