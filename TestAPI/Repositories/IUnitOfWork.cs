using TestAPI.Database;

namespace TestAPI.Repositories;

public interface IUnitOfWork : IDisposable
{
    IParticipantRepository Participants { get; }
    IEventRepository Events { get; }
    Task<int> CompleteAsync();
}