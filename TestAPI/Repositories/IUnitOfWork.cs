using TestAPI.Database;
using TestAPI.Models;

namespace TestAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
    IParticipantRepository Participants { get; }
    IEventRepository Events { get; }
    Task<int> CompleteAsync();

    Task<IEnumerable<Participant>> GetAllByEventAsync(int eventId);

    Task<IEnumerable<Event>> GetAllByParticipantAsync(int participantId);
    Task<bool> RegisterParticipantForEventAsync(int participantId, int eventId);
    Task<bool> CancelParticipantFromEventAsync(int participantId, int eventId);
    Task<DateTime?> GetRegistrationTimeAsync(int participantId, int eventId);
}