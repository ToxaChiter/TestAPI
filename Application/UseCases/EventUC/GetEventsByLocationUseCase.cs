using Application.DTOs;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.EventUC;

public class GetEventsByLocationUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventsByLocationUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<EventDTO>> ExecuteAsync(string location)
    {
        var events = await _unitOfWork.Events.GetAllByLocationAsync(location);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());

        return eventsDTO;
    }
}
