using Application.DTOs;
using Application.Pagination;
using Application.UseCases;
using Application.UseCases.ParticipantUC;
using AutoMapper;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipantController : ControllerBase
{
    private readonly GetAllParticipantsUseCase _getAllParticipantsUseCase;
    private readonly GetParticipantByIdUseCase _getParticipantByIdUseCase;
    private readonly GetEventParticipantsUseCase _getEventParticipantsUseCase;
    private readonly RegisterParticipantUseCase _registerParticipantUseCase;
    private readonly CancelParticipantUseCase _cancelParticipantUseCase;
    private readonly GetRegistrationTimeUseCase _getRegistrationTimeUseCase;

    public ParticipantController(
        GetAllParticipantsUseCase getAllParticipantsUseCase,
        GetParticipantByIdUseCase getParticipantByIdUseCase,
        GetEventParticipantsUseCase getEventParticipantsUseCase,
        RegisterParticipantUseCase registerParticipantUseCase,
        CancelParticipantUseCase cancelParticipantUseCase,
        GetRegistrationTimeUseCase getRegistrationTimeUseCase)
    {
        _getAllParticipantsUseCase = getAllParticipantsUseCase;
        _getParticipantByIdUseCase = getParticipantByIdUseCase;
        _getEventParticipantsUseCase = getEventParticipantsUseCase;
        _registerParticipantUseCase = registerParticipantUseCase;
        _cancelParticipantUseCase = cancelParticipantUseCase;
        _getRegistrationTimeUseCase = getRegistrationTimeUseCase;
    }

    [HttpGet("GetParticipants")]
    public async Task<IActionResult> GetAllAsync([FromQuery] PaginationParams paginationParams)
    {
        var participantsDTO = await _getAllParticipantsUseCase.ExecuteAsync();
        var paginatedParticipantDTO = participantsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedParticipantDTO);
    }

    [HttpGet("GetParticipant/{id}")]
    public async Task<IActionResult> GetByIdAsync(int id)
    {
        var participantDTO = await _getParticipantByIdUseCase.ExecuteAsync(id);
        return Ok(participantDTO);
    }

    [HttpGet("GetParticipantsByEvent")]
    public async Task<IActionResult> GetAllByEventAsync([FromQuery] PaginationParams paginationParams, [FromQuery] int eventId)
    {
        var participantsDTO = await _getEventParticipantsUseCase.ExecuteAsync(eventId);
        var paginatedParticipantDTO = participantsDTO.AsQueryable().Paginate(paginationParams.PageNumber, paginationParams.PageSize);
        return Ok(paginatedParticipantDTO);
    }



    [HttpPost("RegisterParticipant/{participantId}")]
    [Authorize]
    public async Task<IActionResult> RegisterParticipantForEventAsync(int participantId, [FromQuery] int eventId)
    {
        await _registerParticipantUseCase.ExecuteAsync(participantId, eventId);
        return Ok();
    }

    [HttpPost("CancelParticipant/{participantId}")]
    [Authorize]
    public async Task<IActionResult> CancelParticipantFromEventAsync(int participantId, [FromQuery] int eventId)
    {
        await _cancelParticipantUseCase.ExecuteAsync(participantId, eventId);
        return Ok();
    }

    [HttpGet("GetRegistrationTime/{participantId}")]
    [Authorize]
    public async Task<IActionResult> GetRegistrationTimeAsync(int participantId, [FromQuery] int eventId)
    {
        var regTime = await _getRegistrationTimeUseCase.ExecuteAsync(participantId, eventId);
        return Ok(regTime);
    }
}
