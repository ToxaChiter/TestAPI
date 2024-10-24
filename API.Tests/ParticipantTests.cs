using Moq;

namespace API.Tests;

using Application.DTOs;
using Application.UseCases.ParticipantUC;
using AutoMapper;
using Core.Entities;
using Infrastructure.Data.Database;
using Infrastructure.Data.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

public class ParticipantTests
{
    private readonly DbContextOptions<EventDbContext> _options;

    public ParticipantTests()
    {
        _options = new DbContextOptionsBuilder<EventDbContext>()
                        .UseInMemoryDatabase(databaseName: "TestDatabase")
                        .Options;

        using var dbContext = new EventDbContext(_options);
        dbContext.Database.EnsureCreatedAsync();
    }

    [Fact]
    public async Task GetAllParticipantsUseCase_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();

        mapper
            .Setup(c => c.ProjectTo<ParticipantDTO>(
                It.IsAny<IQueryable<Participant>>(),
                It.IsAny<object>(),
                It.Is<Expression<Func<ParticipantDTO, object>>[]>(x => x.Length == 0)))
            .Returns(new List<ParticipantDTO>()
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com" },
                new() { Id = 2, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com" },
            }
            .AsQueryable());

        var getAllParticipantsUseCase = new GetAllParticipantsUseCase(mapper.Object, unitOfWork);

        var participants = new List<Participant>()
        {
            new() { Id = 1, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com", UserId = string.Empty },
            new() { Id = 2, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com", UserId = string.Empty },
        };

        await dbContext.Participants.AddRangeAsync(participants);
        await dbContext.SaveChangesAsync();

        // Act
        var participantsDTO = await getAllParticipantsUseCase.ExecuteAsync();

        // Assert
        Assert.NotNull(participantsDTO);
        Assert.Equal(2, participantsDTO.Count());
        Assert.Equal("John", participantsDTO.First().FirstName);
        Assert.Equal("Jane", participantsDTO.Skip(1).First().FirstName);
    }

    [Fact]
    public async Task GetParticipantById_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();

        var getParticipantByIdUseCase = new GetParticipantByIdUseCase(mapper.Object, unitOfWork);

        var participant = new Participant
        {
            Id = 3,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        mapper.Setup(m => m.Map<ParticipantDTO>(participant))
            .Returns(new ParticipantDTO
            {
                Id = 3,
                FirstName = "John",
                LastName = "Doe",
                DateOfBirth = new DateOnly(2000, 10, 14),
                Email = "john@example.com",
            });

        await dbContext.Participants.AddAsync(participant);
        await dbContext.SaveChangesAsync();

        // Act
        var participantDTO = await getParticipantByIdUseCase.ExecuteAsync(participant.Id);

        // Assert
        Assert.NotNull(participantDTO);
        Assert.Equal("John", participantDTO.FirstName);
    }

    [Fact]
    public async Task GetParticipantsByEventId_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);

        var unitOfWork = new UnitOfWork(dbContext);
        var mapper = new Mock<IMapper>();

        mapper
            .Setup(c => c.ProjectTo<ParticipantDTO>(
                It.IsAny<IQueryable<Participant>>(),
                It.IsAny<object>(),
                It.Is<Expression<Func<ParticipantDTO, object>>[]>(x => x.Length == 0)))
            .Returns(new List<ParticipantDTO>()
            {
                new() { Id = 4, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com" },
                new() { Id = 5, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com" },
            }
            .AsQueryable());

        var getEventParticipantsUseCase = new GetEventParticipantsUseCase(mapper.Object, unitOfWork);

        var participants = new List<Participant>()
        {
            new() { Id = 4, FirstName = "John", LastName = "Doe", DateOfBirth = new DateOnly(2000, 10, 14), Email = "john@example.com", UserId = string.Empty },
            new() { Id = 5, FirstName = "Jane", LastName = "Doe", DateOfBirth = new DateOnly(2001, 6, 27), Email = "jane@example.com", UserId = string.Empty },
        };

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
        var participantsDTO = await getEventParticipantsUseCase.ExecuteAsync(@event.Id);

        // Assert
        Assert.NotNull(participantsDTO);
        Assert.Equal(2, participantsDTO.Count());
        Assert.Equal("John", participantsDTO.First().FirstName);
        Assert.Equal("Jane", participantsDTO.Skip(1).First().FirstName);
    }

    [Fact]
    public async Task RegisterParticipant_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);

        var unitOfWork = new UnitOfWork(dbContext);

        var participant = new Participant
        {
            Id = 6,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        var @event = new Event
        {
            Id = 2,
            Title = "Test Event",
            MaxParticipants = 100,
            EventDateTime = new DateTime(2024, 10, 12),
            Location = "Zen Garden",
        };

        await unitOfWork.Participants.CreateAsync(participant);
        await unitOfWork.Events.CreateAsync(@event);
        await unitOfWork.CompleteAsync();

        var registerParticipantUseCase = new RegisterParticipantUseCase(unitOfWork);

        // Act
        await registerParticipantUseCase.ExecuteAsync(participant.Id, @event.Id);

        // Assert
        Assert.True(dbContext.ParticipantEvents.Any(pe => pe.EventId == @event.Id && pe.ParticipantId == participant.Id));
    }

    [Fact]
    public async Task CancelParticipantRegistration_SuccessAsync()
    {
        // Arrange
        using var dbContext = new EventDbContext(_options);

        var unitOfWork = new UnitOfWork(dbContext);

        var cancelParticipantUseCase = new CancelParticipantUseCase(unitOfWork);

        var participant = new Participant
        {
            Id = 7,
            FirstName = "John",
            LastName = "Doe",
            DateOfBirth = new DateOnly(2000, 10, 14),
            Email = "john@example.com",
            UserId = string.Empty,
        };

        await dbContext.Participants.AddAsync(participant);
        await dbContext.SaveChangesAsync();

        var @event = new Event()
        {
            Id = 3,
            Title = "Test Event",
            MaxParticipants = 100,
            EventDateTime = new DateTime(2024, 10, 12),
            Location = "Zen Garden",
        };

        await dbContext.Events.AddAsync(@event);
        await dbContext.SaveChangesAsync();

        var participantEvent = new ParticipantEvent() 
        { 
            EventId = @event.Id, 
            ParticipantId = participant.Id, 
            RegistrationDateTime = DateTime.UtcNow, 
        };

        await dbContext.ParticipantEvents.AddAsync(participantEvent);
        await dbContext.SaveChangesAsync();

        // Act
        await cancelParticipantUseCase.ExecuteAsync(@event.Id, participant.Id);

        // Assert
        Assert.False(dbContext.ParticipantEvents.Any(pe => pe.EventId == @event.Id && pe.ParticipantId == participant.Id));
    }
}
