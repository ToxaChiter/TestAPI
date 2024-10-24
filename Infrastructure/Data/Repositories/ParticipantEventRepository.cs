using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class ParticipantEventRepository : Repository<ParticipantEvent>, IParticipantEventRepository
{
    public ParticipantEventRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<int> GetCountByEventAsync(int eventId)
    {
        return await _context.ParticipantEvents.Where(pe => pe.EventId == eventId).CountAsync();
    }

    public async Task<ParticipantEvent?> GetByParticipantAndEventAsync(int participantId, int eventId)
    {
        return await _context.ParticipantEvents.FirstOrDefaultAsync(pe => pe.EventId == eventId && pe.ParticipantId == participantId);
    }
}
