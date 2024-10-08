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
    public async Task<IActionResult> GetAsync([FromQuery] PaginationParams paginationParams)
    {
        var participants = await _unitOfWork.Participants.GetAllAsync();
        var participantsDTO = _mapper.ProjectTo<ParticipantDTO>(participants.AsQueryable());
        var paginatedParticipantDTO = participantsDTO.Paginate(paginationParams.PageNumber, paginationParams.PageSize);
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

    //[HttpPost("PostParticipant")]
    //public async Task<IActionResult> PostAsync([FromBody] ParticipantDTO participantDTO)
    //{
    //    var validationResult = await _validator.ValidateAsync(participantDTO);
    //    if (!validationResult.IsValid)
    //    {
    //        return ValidationProblem();
    //    }
    //    var participant = _mapper.Map<Participant>(participantDTO);

    //    var created = await _unitOfWork.Participants.CreateAsync(participant);
    //    await _unitOfWork.CompleteAsync();

    //    if (created)
    //    {
    //        return Created();
    //    }
    //    else
    //    {
    //        return Problem();
    //    }
    //}
}
