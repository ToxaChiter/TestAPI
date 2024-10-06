
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TestAPI.Database;
using TestAPI.Repositories;

namespace TestAPI;

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

        builder.Services.AddScoped<UnitOfWork>();

        builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();

        //builder.Services.AddAuthentication("Bearer").AddJwtBearer();
        //builder.Services.AddAuthorization();

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();


        app.MapControllers();

        app.Run();
    }
}
