using Application.DTOs;
using AutoMapper;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.UseCases.EventUC;

public class GetEventByIdUseCase
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public GetEventByIdUseCase(IMapper mapper, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<EventDTO> ExecuteAsync(int id)
    {
        var @event = await _unitOfWork.Events.GetByIdAsync(id);
        if (@event is null)
        {
            throw new NotFoundException("Event not found");
        }

        var eventDTO = _mapper.Map<EventDTO>(@event);

        return eventDTO;
    }
}
