using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TestAPI.Models;
using TestAPI.Repositories;

namespace TestAPI.Controllers;

public class LoginController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IValidator<UserRegistration> _validatorReg;
    private readonly IValidator<UserLogin> _validatorLogin;
    private readonly IUnitOfWork _unitOfWork;

    //public LoginController(IMapper mapper, UserManager<User> userManager, IValidator<UserRegistration> validatorReg, IValidator<UserLogin> validatorLogin, IUnitOfWork unitOfWork)
    //{
    //    _mapper = mapper;
    //    _userManager = userManager;
    //    _validatorReg = validatorReg;
    //    _validatorLogin = validatorLogin;
    //    _unitOfWork = unitOfWork;
    //}

    public LoginController(IMapper mapper, UserManager<User> userManager, SignInManager<User> signInManager, IValidator<UserRegistration> validatorReg, IValidator<UserLogin> validatorLogin, IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _validatorReg = validatorReg;
        _validatorLogin = validatorLogin;
        _unitOfWork = unitOfWork;
    }

    [HttpPost("Reg")]
    public async Task<IActionResult> Register([FromBody] UserRegistration userRegistration)
    {
        var validationResult = await _validatorReg.ValidateAsync(userRegistration);
        if (!validationResult.IsValid)
        {
            return ValidationProblem();
        }
        var user = _mapper.Map<User>(userRegistration);

        var result = await _userManager.CreateAsync(user, userRegistration.Password);
        if (!result.Succeeded)
        {
            return Problem(string.Join(", ", result.Errors.Select(err => err.Description)));
        }

        var userId = (await _userManager.FindByEmailAsync(userRegistration.Email)).Id;
        Participant participant = new()
        {
            Email = userRegistration.Email,
            FirstName = userRegistration.FirstName,
            LastName = userRegistration.LastName,
            UserId = userId,
            DateOfBirth = userRegistration.DateOfBirth,
        };

        await _unitOfWork.Participants.CreateAsync(participant);

        await _userManager.AddToRoleAsync(user, "User");

        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        var validationResult = await _validatorLogin.ValidateAsync(userLogin);
        if (!validationResult.IsValid)
        {
            return ValidationProblem();
        }

        var user = await _userManager.FindByEmailAsync(userLogin.Email);
        if (user != null && await _userManager.CheckPasswordAsync(user, userLogin.Password))
        {
            await _signInManager.SignOutAsync();
            await _signInManager.SignInAsync(user, isPersistent: false);

            return Ok();
        }
        else
        {
            return BadRequest("Invalid UserName or Password");
        }
    }

    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return Ok();
    }
}
