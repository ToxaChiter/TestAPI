using Application.Services;
using Core.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class LoginController : ControllerBase
{
    private readonly LoginService _loginService;

    public LoginController(LoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpPost("Reg")]
    public async Task<IActionResult> Register([FromBody] UserRegistration userRegistration)
    {
        await _loginService.Register(userRegistration);
        return Ok();
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] UserLogin userLogin)
    {
        await _loginService.Login(userLogin);
        return Ok();
    }

    [HttpPost("Logout")]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        await _loginService.Logout();
        return Ok();
    }
}