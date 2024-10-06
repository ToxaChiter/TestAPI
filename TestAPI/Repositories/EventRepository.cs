using Microsoft.EntityFrameworkCore;
using TestAPI.Database;
using TestAPI.Models;

namespace TestAPI.Repositories;

public class EventRepository : Repository<Event>, IEventRepository
{
    public EventRepository(EventDbContext context) : base(context)
    {
    }

    public async Task<Event?> GetByNameAsync(string name)
    {
        return await _context.Events.FindAsync(name);
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
            .Where(e => e.Location.Equals(location))
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllByCategoryAsync(string category)
    {
        return await _context.Events
            .Where(e => e.Category.Equals(category))
            .ToListAsync();
    }
}
