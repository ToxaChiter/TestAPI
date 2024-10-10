using Microsoft.EntityFrameworkCore;
using TestAPI.Database;
using TestAPI.Models;

namespace TestAPI.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<Event?> GetByNameAsync(string title)
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

    public async Task<IEnumerable<Event>> GetAllByParticipantAsync(Participant participant)
    {
        return await _context.Events
            .Where(e => e.Participants.Contains(participant))
            .ToListAsync();
    }
}
