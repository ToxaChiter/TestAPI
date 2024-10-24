using Application.DTOs;
using Application.Pagination;
using Application.UseCases.EventUC;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class EventController : ControllerBase
{
    private readonly GetAllEventsUseCase _getAllEventsUseCase;
    private readonly GetEventByIdUseCase _getEventByIdUseCase;
    private readonly GetEventByTitleUseCase _getEventByTitleUseCase;
    private readonly GetEventsByDateUseCase _getEventsByDateUseCase;
    private readonly GetEventsByLocationUseCase _getEventsByLocationUseCase;
    private readonly GetEventsByCategoryUseCase _getEventsByCategoryUseCase;
    private readonly GetEventsByParticipantUseCase _getEventsByParticipantUseCase;
    private readonly CreateEventUseCase _createEventUseCase;
    private readonly UpdateEventUseCase _updateEventUseCase;
    private readonly DeleteEventUseCase _deleteEventUseCase;
    private readonly SetEventImageUseCase _setEventImageUseCase;

    public EventController(
        GetAllEventsUseCase getAllEventsUseCase,
        GetEventByIdUseCase getEventByIdUseCase,
        GetEventByTitleUseCase getEventByTitleUseCase,
        GetEventsByDateUseCase getEventsByDateUseCase,
        GetEventsByLocationUseCase getEventsByLocationUseCase,
        GetEventsByCategoryUseCase getEventsByCategoryUseCase,
        GetEventsByParticipantUseCase getEventsByParticipantUseCase,
        CreateEventUseCase createEventUseCase,
        UpdateEventUseCase updateEventUseCase,
        DeleteEventUseCase deleteEventUseCase,
        SetEventImageUseCase setEventImageUseCase)
    {
        _getAllEventsUseCase = getAllEventsUseCase;
        _getEventByIdUseCase = getEventByIdUseCase;
        _getEventByTitleUseCase = getEventByTitleUseCase;
        _getEventsByDateUseCase = getEventsByDateUseCase;
        _getEventsByLocationUseCase = getEventsByLocationUseCase;
        _getEventsByCategoryUseCase = getEventsByCategoryUseCase;
        _getEventsByParticipantUseCase = getEventsByParticipantUseCase;
        _createEventUseCase = createEventUseCase;
        _updateEventUseCase = updateEventUseCase;
        _deleteEventUseCase = deleteEventUseCase;
        _setEventImageUseCase = setEventImageUseCase;
    }

    [HttpGet("GetEvents")]
    public async Task<IActionResult> GetAsync([FromQuery] PaginationParams paginationParams)
    {
        var eventsDTO = await _getAllEventsUseCase.ExecuteAsync();
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEvent/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var eventDTO = await _getEventByIdUseCase.ExecuteAsync(id);
        return Ok(eventDTO);
    }

    [HttpGet("GetEventByTitle")]
    public async Task<IActionResult> GetByTitleAsync([FromQuery] string title)
    {
        var eventDTO = await _getEventByTitleUseCase.ExecuteAsync(title);
        return Ok(eventDTO);
    }

    [HttpGet("GetEventsByDate")]
    public async Task<IActionResult> GetAllByDateAsync([FromQuery] DateOnly date, [FromQuery] PaginationParams paginationParams)
    {
        var eventsDTO = await _getEventsByDateUseCase.ExecuteAsync(date);
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByLocation")]
    public async Task<IActionResult> GetAllByLocationAsync([FromQuery] string location, [FromQuery] PaginationParams paginationParams)
    {
        var eventsDTO = await _getEventsByLocationUseCase.ExecuteAsync(location);
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByCategory")]
    public async Task<IActionResult> GetAllByCategoryAsync([FromQuery] string category, [FromQuery] PaginationParams paginationParams)
    {
        var eventsDTO = await _getEventsByCategoryUseCase.ExecuteAsync(category);
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpGet("GetEventsByParticipant")]
    [Authorize]
    public async Task<IActionResult> GetAllByParticipantAsync([FromQuery] int participantId, [FromQuery] PaginationParams paginationParams)
    {
        var eventsDTO = await _getEventsByParticipantUseCase.ExecuteAsync(participantId);
        var paginatedEventDTO = eventsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedEventDTO);
    }

    [HttpPost("CreateEvent")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<IActionResult> CreateEventAsync([FromBody] EventDTO eventDTO)
    {
        await _createEventUseCase.ExecuteAsync(eventDTO);
        return Created();
    }

    [HttpPost("UpdateEvent")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> UpdateEventAsync([FromBody] EventDTO eventDTO)
    {
        var updated = await _updateEventUseCase.ExecuteAsync(eventDTO);
        return Ok(updated);
    }

    [HttpDelete("DeleteEvent/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetEventImageAsync([FromBody] EventDTO eventDTO)
    {
        await _deleteEventUseCase.ExecuteAsync(eventDTO);
        return NoContent();
    }

    [HttpPut("SetEventImage/{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SetEventImageAsync(int id, IFormFile file)
    {
        await _setEventImageUseCase.ExecuteAsync(id, file);
        return Created();
    }
}
