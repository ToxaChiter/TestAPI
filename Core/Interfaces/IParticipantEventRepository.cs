using Core.Entities;

namespace Core.Interfaces;

public interface IParticipantEventRepository : IRepository<ParticipantEvent>
{
    Task<int> GetCountByEventAsync(int eventId);
    Task<ParticipantEvent?> GetByParticipantAndEventAsync(int participantId, int eventId);
}
