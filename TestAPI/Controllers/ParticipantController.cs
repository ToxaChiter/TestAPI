using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TestAPI.Models;
using TestAPI.Repositories;

namespace TestAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class ParticipantController : ControllerBase
{
    private readonly IValidator<Participant> _validator;
    private readonly UnitOfWork _unitOfWork;

    public ParticipantController(IValidator<Participant> validator, UnitOfWork unitOfWork)
    {
        _validator = validator;
        _unitOfWork = unitOfWork;
    }



    //[HttpGet]
    //public IEnumerable<Participant> Get()
    //{
    //    return new string[] { "value1", "value2" };
    //}

    //[HttpGet("{id}")]
    //public Participant Get(int id)
    //{
    //    return "value";
    //}

    //[HttpPost]
    //public void Post([FromBody] ParticipantDTO value)
    //{
    //}

    //[HttpPut("{id}")]
    //public void Put(int id, [FromBody] Participant value)
    //{
    //}

    //[HttpDelete("{id}")]
    //public void Delete(int id)
    //{
    //}
}
