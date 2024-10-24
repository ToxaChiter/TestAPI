using Core.Entities;

namespace Core.Interfaces;

public interface IParticipantRepository : IRepository<Participant>
{
    Task<IEnumerable<Participant>> GetAllFromEventAsync(int eventId);
}
