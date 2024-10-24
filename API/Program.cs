using API.Middleware;
using Application.Mapping.Profiles;
using Application.Services;
using Application.UseCases;
using Application.UseCases.EventUC;
using Application.UseCases.ParticipantUC;
using Application.Validation;
using Core.Entities;
using Core.Interfaces;
using FluentValidation;
using Infrastructure.Data.Database;
using Infrastructure.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddDbContext<EventDbContext>(optionsBuilder =>
        {
            optionsBuilder.UseSqlServer(builder.Configuration["ConnectionStrings:DefaultDBConnection"]);
        });

        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

        builder.Services.AddScoped<LoginService, LoginService>();
        builder.Services.AddScoped<ImageService, ImageService>();

        builder.Services.AddScoped<GetAllParticipantsUseCase, GetAllParticipantsUseCase>();
        builder.Services.AddScoped<GetParticipantByIdUseCase, GetParticipantByIdUseCase>();
        builder.Services.AddScoped<GetEventParticipantsUseCase, GetEventParticipantsUseCase>();
        builder.Services.AddScoped<RegisterParticipantUseCase, RegisterParticipantUseCase>();
        builder.Services.AddScoped<CancelParticipantUseCase, CancelParticipantUseCase>();

        builder.Services.AddScoped<GetRegistrationTimeUseCase, GetRegistrationTimeUseCase>();

        builder.Services.AddScoped<GetAllEventsUseCase, GetAllEventsUseCase>();
        builder.Services.AddScoped<GetEventByIdUseCase, GetEventByIdUseCase>();
        builder.Services.AddScoped<GetEventByTitleUseCase, GetEventByTitleUseCase>();
        builder.Services.AddScoped<GetEventsByDateUseCase, GetEventsByDateUseCase>();
        builder.Services.AddScoped<GetEventsByLocationUseCase, GetEventsByLocationUseCase>();
        builder.Services.AddScoped<GetEventsByCategoryUseCase, GetEventsByCategoryUseCase>();
        builder.Services.AddScoped<GetEventsByParticipantUseCase, GetEventsByParticipantUseCase>();
        builder.Services.AddScoped<CreateEventUseCase, CreateEventUseCase>();
        builder.Services.AddScoped<UpdateEventUseCase, UpdateEventUseCase>();
        builder.Services.AddScoped<DeleteEventUseCase, DeleteEventUseCase>();
        builder.Services.AddScoped<SetEventImageUseCase, SetEventImageUseCase>();


        builder.Services.AddAutoMapper(typeof(ParticipantProfile));
        builder.Services.AddValidatorsFromAssemblyContaining<ParticipantDTOValidator>();

        builder.Services.AddIdentity<User, IdentityRole>(options =>
        {
            options.Password.RequiredLength = 8;
            options.Password.RequireDigit = false;
            options.Password.RequireUppercase = false;
            options.Password.RequireNonAlphanumeric = false;

            options.User.RequireUniqueEmail = true;
        })
            //.AddRoles<IdentityRole>()
            .AddEntityFrameworkStores<EventDbContext>()
            .AddDefaultTokenProviders();

        builder.Services.AddAuthentication()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? "DemoKey"))
            };
        });

        builder.Services.AddAuthorizationBuilder()
            .AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

        builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options =>
        {
            options.LoginPath = "/Login";
        });

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler("/Error");
            app.UseHsts();

            app.UseMiddleware<ExceptionMiddleware>();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();

            app.UseMiddleware<ExceptionMiddleware>();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
