using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<Event?> GetByTitleAsync(string title)
    {
        return await _context.Events.FirstOrDefaultAsync(ev => ev.Title == title);
    }

    public async Task<IEnumerable<Event>> GetAllByDateAsync(DateOnly date)
    {
        return await _context.Events
            .Where(e => e.EventDateTime.Date == date.ToDateTime(TimeOnly.MinValue))
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllByLocationAsync(string location)
    {
        return await _context.Events
            .Where(e => location.Equals(e.Location))
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllByCategoryAsync(string category)
    {
        return await _context.Events
            .Where(e => category.Equals(e.Category))
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllByParticipantAsync(int participantId)
    {
        return await _context.Events
            .Where(e => e.Participants.Any(p => p.Id == participantId))
            .ToListAsync();
    }
}
