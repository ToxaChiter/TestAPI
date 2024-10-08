using TestAPI.Models;

namespace TestAPI.Repositories;

public interface IEventRepository : IRepository<Event>
{
    Task<Event?> GetByNameAsync(string name);

    // pattern Specification?

    Task<IEnumerable<Event>> GetAllByDateAsync(DateOnly date);
    Task<IEnumerable<Event>> GetAllByLocationAsync(string location);
    Task<IEnumerable<Event>> GetAllByCategoryAsync(string category);

    Task<IEnumerable<Event>> GetAllByParticipantAsync(Participant participant);
}
