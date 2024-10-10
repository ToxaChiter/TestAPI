using Moq;

namespace TestAPI.Tests;

using Microsoft.EntityFrameworkCore;
using Xunit;
using System;
using TestAPI.Controllers;
using TestAPI.Models;
using TestAPI.Database;
using TestAPI.Repositories;
using AutoMapper;
using FluentValidation;
using TestAPI.DTOs;
using TestAPI.Profiles;

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
    public void RegisterParticipant_Success()
    {
        // Arrange
        using var context = new EventDbContext(_options);

        //var controller = new ParticipantController(Mock<IMapper>)

        Assert.True(true);
    }
}

