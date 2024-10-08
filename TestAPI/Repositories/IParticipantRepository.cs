using TestAPI.Models;

namespace TestAPI.Repositories;

public interface IParticipantRepository : IRepository<Participant>
{
    Task<IEnumerable<Participant>> GetAllFromEventAsync(Event @event);
}
