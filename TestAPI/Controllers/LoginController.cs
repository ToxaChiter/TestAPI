using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TestAPI.DTOs;
using TestAPI.Models;
using TestAPI.Repositories;

namespace TestAPI.Controllers;

public class LoginController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly IValidator<UserDTO> _validator;
    private readonly IUnitOfWork _unitOfWork;

    public LoginController(IMapper mapper, UserManager<User> userManager, IValidator<UserDTO> validator, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = userManager;
        _validator = validator;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("Reg")]
    public async Task<IActionResult> Register([FromBody] UserDTO userDTO)
    {
        var validationResult = await _validator.ValidateAsync(userDTO);
        if (!validationResult.IsValid)
        {
            return ValidationProblem();
        }
        var user = _mapper.Map<User>(userDTO);

        var result = await _userManager.CreateAsync(user, user.Password);
        if (!result.Succeeded)
        {
            return Problem(string.Join(", ", result.Errors.Select(err => err.Description)));
        }

        await _userManager.AddToRoleAsync(user, "User");

        return Ok();
    }
}
