using Core.Entities;

namespace Core.Interfaces;

public interface IEventRepository : IRepository<Event>
{
    Task<Event?> GetByTitleAsync(string name);

    // pattern Specification?

    Task<IEnumerable<Event>> GetAllByDateAsync(DateOnly date);
    Task<IEnumerable<Event>> GetAllByLocationAsync(string location);
    Task<IEnumerable<Event>> GetAllByCategoryAsync(string category);

    Task<IEnumerable<Event>> GetAllByParticipantAsync(int participantId);
}
