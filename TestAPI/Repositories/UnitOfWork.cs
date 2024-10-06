
using TestAPI.Database;

namespace TestAPI.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventDbContext _dbContext;

    public UnitOfWork(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IParticipantRepository _participantRepository; // = null!
    private IEventRepository _eventRepository;

    public IParticipantRepository Participants
    {
        get
        {
            _participantRepository ??= new ParticipantRepository(_dbContext);
            return _participantRepository;
        }
    }
    public IEventRepository Events
    {
        get
        {
            _eventRepository ??= new EventRepository(_dbContext);
            return _eventRepository;
        }
    }

    public Task<int> CompleteAsync()
    {
        return _dbContext.SaveChangesAsync();
    }


    private bool disposed = false;

    public virtual void Dispose(bool disposing)
    {
        if (!this.disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            this.disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}
