using Core.Entities;

namespace Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IParticipantRepository Participants { get; }
    IEventRepository Events { get; }
    IParticipantEventRepository ParticipantEvents { get; }
    Task<int> CompleteAsync();

    Task<IEnumerable<Participant>> GetAllByEventAsync(int eventId);

    Task<IEnumerable<Event>> GetAllByParticipantAsync(int participantId);
}