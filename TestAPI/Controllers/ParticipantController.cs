using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.Entity;
using TestAPI.DTOs;
using TestAPI.Models;
using TestAPI.Pagination;
using TestAPI.Repositories;

namespace TestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipantController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IValidator<ParticipantDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public ParticipantController(IMapper mapper, IValidator<ParticipantDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    [HttpGet("GetParticipants")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
    {
        var participants = await _unitOfWork.Participants.GetAllAsync();
        var participantsDTO = _mapper.ProjectTo<ParticipantDTO>(participants.AsQueryable());
        var paginatedParticipantDTO = participantsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedParticipantDTO);
    }

    [HttpGet("GetParticipantsByEvent")]
    public async Task<IActionResult> GetAllByEventAsync([FromQuery] PaginationParams paginationParams, [FromQuery] int eventId)
    {
        var participants = await _unitOfWork.Participants.GetAllFromEventAsync(eventId);
        var participantsDTO = _mapper.ProjectTo<ParticipantDTO>(participants.AsQueryable());
        var paginatedParticipantDTO = participantsDTO.AsQueryable().Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedParticipantDTO);
    }

    [HttpGet("GetParticipant/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var participant = await _unitOfWork.Participants.GetByIdAsync(id);

        if (participant is null)
        {
            return NotFound();
        }

        var participantDTO = _mapper.Map<ParticipantDTO>(participant);
        return Ok(participantDTO);
    }

    [HttpPost("RegisterParticipant/{participantId}")]
    [Authorize]
    public async Task<IActionResult> RegisterParticipantForEventAsync(int participantId, [FromQuery] int eventId)
    {
        var result = await _unitOfWork.RegisterParticipantForEventAsync(participantId, eventId);

        if (result is true)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpPost("CancelParticipant/{participantId}")]
    [Authorize]
    public async Task<IActionResult> CancelParticipantFromEventAsync(int participantId, [FromQuery] int eventId)
    {
        var result = await _unitOfWork.CancelParticipantFromEventAsync(participantId, eventId);

        if (result is true)
        {
            return Ok();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet("GetRegistrationTime/{participantId}")]
    [Authorize]
    public async Task<IActionResult> GetRegistrationTimeAsync(int participantId, [FromQuery] int eventId)
    {
        var dateTime = await _unitOfWork.GetRegistrationTimeAsync(participantId, eventId);

        if (dateTime is null)
        {
            return BadRequest();
        }
        else
        {
            return Ok(dateTime);
        }
    }
}
