using Microsoft.EntityFrameworkCore;
using System;
using TestAPI.Database;
using TestAPI.Models;

namespace TestAPI.Repositories;



public class ParticipantRepository : Repository<Participant>, IParticipantRepository
{
    public ParticipantRepository(EventDbContext context) : base(context)
    {
    }
}
