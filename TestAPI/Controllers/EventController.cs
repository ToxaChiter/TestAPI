using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestAPI.DTOs;
using TestAPI.Models;
using TestAPI.Pagination;
using TestAPI.Repositories;

namespace TestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IValidator<EventDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public EventController(IMapper mapper, IValidator<EventDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationParams paginationParams)
    {
        var events = await _unitOfWork.Events.GetAllAsync();
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEvent/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var @event = await _unitOfWork.Events.GetByIdAsync(id);

        if (@event is null)
        {
            return NotFound();
        }

        var eventDTO = _mapper.Map<EventDTO>(@event);
        return Ok(eventDTO);
    }

    [HttpGet("GetEventByTitle")]
    public async Task<IActionResult> GetByTitleAsync([FromQuery] string title)
    {
        var @event = await _unitOfWork.Events.GetByNameAsync(title);

        if (@event is null)
        {
            return NotFound();
        }

        var eventDTO = _mapper.Map<EventDTO>(@event);
        return Ok(eventDTO);
    }

    [HttpGet("GetEventsByDate")]
    public async Task<IActionResult> GetAllByDateAsync([FromQuery] DateOnly date, [FromQuery] PaginationParams paginationParams)
    {
        var events = await _unitOfWork.Events.GetAllByDateAsync(date);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByLocation")]
    public async Task<IActionResult> GetAllByLocationAsync([FromQuery] string location, [FromQuery] PaginationParams paginationParams)
    {
        var events = await _unitOfWork.Events.GetAllByLocationAsync(location);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByCategory")]
    public async Task<IActionResult> GetAllByCategoryAsync([FromQuery] string category, [FromQuery] PaginationParams paginationParams)
    {
        var events = await _unitOfWork.Events.GetAllByLocationAsync(category);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByParticipant")]
    [Authorize]
    public async Task<IActionResult> GetAllByParticipantAsync([FromQuery] int participantId, [FromQuery] PaginationParams paginationParams)
    {
        var events = await _unitOfWork.GetAllByParticipantAsync(participantId);
        var eventsDTO = _mapper.ProjectTo<EventDTO>(events.AsQueryable());
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpPost("CreateEvent")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventDTO eventDTO)
    {
        var validationResult = await _validator.ValidateAsync(eventDTO);
        if (!validationResult.IsValid)
        {
            return ValidationProblem();
        }
        var @event = _mapper.Map<Event>(eventDTO);

        var isCreated = await _unitOfWork.Events.CreateAsync(@event);

        if (isCreated)
        {
            return Ok();
        }
        else
        {
            return Problem();
        }
    }

    [HttpPost("UpdateEvent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEventAsync([FromBody] EventDTO eventDTO)
    {
        var validationResult = await _validator.ValidateAsync(eventDTO);
        if (!validationResult.IsValid)
        {
            return ValidationProblem();
        }
        var @event = _mapper.Map<Event>(eventDTO);

        var updated = await _unitOfWork.Events.UpdateAsync(@event);

        if (updated is null)
        {
            return Problem();
        }
        else
        {
            return Ok(updated);
        }
    }

    [HttpDelete("DeleteEvent/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetEventImageAsync(int id)
    {
        var isRemoved = await _unitOfWork.Events.DeleteAsync(id);

        if (isRemoved)
        {
            return NoContent();
        }
        else
        {
            return Problem();
        }
    }

    [HttpPut("SetEventImage/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetEventImageAsync(int id, IFormFile file)
    {
        var @event = await _unitOfWork.Events.GetByIdAsync(id);

        if (@event is null)
        {
            return NotFound();
        }

        var filename = file.FileName;

        @event.ImagePath = filename;
        var updated = await _unitOfWork.Events.UpdateAsync(@event);

        if (updated?.ImagePath == filename)
        {
            return RedirectToAction("Upload", "Image");
        }
        else
        {
            return Problem();
        }
    }
}
