using AutoMapper;
using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class LoginService
{
    private readonly IMapper _mapper;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IValidator<UserRegistration> _validatorReg;
    private readonly IValidator<UserLogin> _validatorLogin;
    private readonly IUnitOfWork _unitOfWork;

    public LoginService(
        IMapper mapper,
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IValidator<UserRegistration> validatorReg,
        IValidator<UserLogin> validatorLogin,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _userManager = userManager;
        _signInManager = signInManager;
        _validatorReg = validatorReg;
        _validatorLogin = validatorLogin;
        _unitOfWork = unitOfWork;
    }

    public async Task Register(UserRegistration userRegistration)
    {
        var validationResult = await _validatorReg.ValidateAsync(userRegistration);
        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Registration data is not valid");
        }
        var user = _mapper.Map<User>(userRegistration);

        var result = await _userManager.CreateAsync(user, userRegistration.Password);
        if (!result.Succeeded)
        {
            throw new Exception(string.Join(", ", result.Errors.Select(err => err.Description)));
        }

        var userDb = await _userManager.FindByEmailAsync(userRegistration.Email);
        if (userDb is null)
        {
            throw new NotFoundException("User is not found");
        }

        var userId = userDb.Id;
        Participant participant = new()
        {
            Email = userRegistration.Email,
            FirstName = userRegistration.FirstName,
            LastName = userRegistration.LastName,
            UserId = userId,
            DateOfBirth = userRegistration.DateOfBirth,
        };

        await _unitOfWork.Participants.CreateAsync(participant);

        var identityResult = await _userManager.AddToRoleAsync(user, "User");
        if (!identityResult.Succeeded)
        {
            throw new Exception("Cannot create user");
        }
    }

    public async Task Login(UserLogin userLogin)
    {
        var validationResult = await _validatorLogin.ValidateAsync(userLogin);
        if (!validationResult.IsValid)
        {
            throw new BadRequestException("Login data is not valid");
        }

        var user = await _userManager.FindByEmailAsync(userLogin.Email);
        if (user is null)
        {
            throw new NotFoundException("User is not found");
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, userLogin.Password);
        if (!isPasswordValid)
        {
            throw new BadRequestException("Invalid password");
        }

        await _signInManager.SignOutAsync();
        await _signInManager.SignInAsync(user, isPersistent: false);
    }

    public async Task Logout()
    {
        await _signInManager.SignOutAsync();
    }
}
