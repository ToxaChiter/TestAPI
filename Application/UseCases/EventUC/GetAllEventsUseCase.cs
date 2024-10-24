using Application.DTOs;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.EventUC;

public class GetAllEventsUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetAllEventsUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<EventDTO>> ExecuteAsync()
    {
        var events = await _unitOfWork.Events.GetAllAsync();
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());

        return eventsDTO;
    }
}
