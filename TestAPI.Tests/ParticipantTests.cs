using Moq;

namespace TestAPI.Tests;

using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;
using TestAPI.Controllers;
using TestAPI.Database;
using TestAPI.DTOs;
using TestAPI.Models;
using TestAPI.Pagination;
using TestAPI.Repositories;
using Xunit;
using Xunit.Sdk;

public class ParticipantTests
{
    private DbContextOptions<EventDbContext> _options;

    public ParticipantTests()
    {
        _options = new DbContextOptionsBuilder<EventDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDatabase")
                        .Options;
    }

    [Fact]
    public async Task RegisterParticipant_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();
        var validator = new Mock<IValidator<ParticipantDTO>>();

        var participant = new Participant
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        var @event = new Event
        {
            Id = 1,
            Title = "Test Event",
            MaxParticipants = 100,
            EventDateTime = new DateTime(2024, 10, 12),
            Location = "Zen Garden",
        };

        await unitOfWork.Participants.CreateAsync(participant);
        await unitOfWork.Events.CreateAsync(@event);
        await unitOfWork.CompleteAsync();

        var controller = new ParticipantController(mapper.Object, validator.Object, unitOfWork);

        // Act
        var result = await controller.RegisterParticipantForEventAsync(participant.Id, @event.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        Assert.True(dbContext.ParticipantEvents.Any());
    }

    [Fact]
    public async Task GetParticipantsByEventId_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();
        var validator = new Mock<IValidator<ParticipantDTO>>();

        var controller = new ParticipantController(mapper.Object, validator.Object, unitOfWork);

        var participants = new List<Participant>()
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com", UserId = string.Empty },
            new() { Id = 2, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com", UserId = string.Empty },
        };

        //mapper.Setup(m => m.ProjectTo<ParticipantDTO>(It.IsAny<IQueryable<Participant>>(), null))
        //    .Returns(new List<ParticipantDTO>()
        //    {
        //        new() { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com" },
        //        new() { Id = 2, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com" },
        //    }
        //    .AsQueryable());

        await dbContext.Participants.AddRangeAsync(participants);

        var @event = new Event()
        {
            Id = 1,
            Title = "Test Event",
            MaxParticipants = 100,
            EventDateTime = new DateTime(2024, 10, 12),
            Location = "Zen Garden",
        };

        await dbContext.Events.AddAsync(@event);
        await dbContext.SaveChangesAsync();

        var participantEvents = participants.Select(p => new ParticipantEvent() { EventId = @event.Id, ParticipantId = p.Id, RegistrationDateTime = DateTime.UtcNow });

        await dbContext.ParticipantEvents.AddRangeAsync(participantEvents);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.GetAllByEventAsync(new PaginationParams() { PageNumber = 1, PageSize = 10 }, @event.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var objectResult = (result as OkObjectResult)?.Value;

        Assert.NotNull(objectResult);
        Assert.IsAssignableFrom<IQueryable<ParticipantDTO>>(objectResult);

        //var participantsDTO = objectResult as IQueryable<ParticipantDTO>;

        //Assert.NotNull(participantsDTO);
        //Assert.Equal(2, participantsDTO.Count());
        //Assert.Equal("John", participantsDTO.First().FirstName);
        //Assert.Equal("Jane", participantsDTO.Skip(1).First().FirstName);
    }

    [Fact]
    public async Task GetParticipantById_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();
        var validator = new Mock<IValidator<ParticipantDTO>>();

        var controller = new ParticipantController(mapper.Object, validator.Object, unitOfWork);

        var participant = new Participant
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        mapper.Setup(m => m.Map<ParticipantDTO>(participant))
            .Returns(new ParticipantDTO
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(2000, 10, 14),
                Email = "john@example.com",
            });

        await dbContext.Participants.AddAsync(participant);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.GetByIdAsync(participant.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkObjectResult>(result);

        var objectResult = (result as OkObjectResult)?.Value;

        Assert.NotNull(objectResult);
        Assert.IsType<ParticipantDTO>(objectResult);

        var participantDTO = objectResult as ParticipantDTO;

        Assert.NotNull(participantDTO);
        Assert.Equal("John", participantDTO.FirstName);
    }

    [Fact]
    public async Task CancelParticipantRegistration_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);
        await dbContext.Database.EnsureDeletedAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();
        var validator = new Mock<IValidator<ParticipantDTO>>();

        var controller = new ParticipantController(mapper.Object, validator.Object, unitOfWork);

        var participant = new Participant
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        await dbContext.Participants.AddAsync(participant);

        var @event = new Event()
        {
            Id = 1,
            Title = "Test Event",
            MaxParticipants = 100,
            EventDateTime = new DateTime(2024, 10, 12),
            Location = "Zen Garden",
        };

        await dbContext.Events.AddAsync(@event);
        await dbContext.SaveChangesAsync();

        var participantEvent = new ParticipantEvent() { EventId = @event.Id, ParticipantId = participant.Id, RegistrationDateTime = DateTime.UtcNow };

        await dbContext.ParticipantEvents.AddAsync(participantEvent);
        await dbContext.SaveChangesAsync();

        // Act
        var result = await controller.CancelParticipantFromEventAsync(participant.Id, @event.Id);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<OkResult>(result);
        Assert.False(dbContext.ParticipantEvents.Any());
    }
}

