﻿using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class ParticipantRepository : Repository<Participant>, IParticipantRepository
{
    public ParticipantRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Participant>> GetAllFromEventAsync(int eventId)
    {
        return await _context.Participants
            .Where(p => p.Events.Any(e => e.Id == eventId))
            .ToListAsync();
    }
}
