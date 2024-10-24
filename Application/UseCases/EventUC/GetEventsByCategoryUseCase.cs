using Application.DTOs;
using AutoMapper;
using Core.Interfaces;

namespace Application.UseCases.EventUC;

public class GetEventsByCategoryUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventsByCategoryUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<IQueryable<EventDTO>> ExecuteAsync(string category)
    {
        var events = await _unitOfWork.Events.GetAllByCategoryAsync(category);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());

        return eventsDTO;
    }
}
