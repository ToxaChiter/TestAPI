using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Database;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly EventDbContext _dbContext;

    public UnitOfWork(EventDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    private IParticipantRepository _participantRepository; // = null!
    private IEventRepository _eventRepository;
    private IParticipantEventRepository _participantEventRepository;

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
    public IParticipantEventRepository ParticipantEvents
    {
        get
        {
            _participantEventRepository ??= new ParticipantEventRepository(_dbContext);
            return _participantEventRepository;
        }
    }

    public Task<int> CompleteAsync()
    {
        return _dbContext.SaveChangesAsync();
    }


    private bool disposed = false;

    public virtual void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
            disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }





    public async Task<IEnumerable<Participant>> GetAllByEventAsync(int eventId)
    {
        return await _dbContext.ParticipantEvents
            .Where(pe => pe.EventId == eventId)
            .Select(pe => pe.Participant!)    // !
            .ToListAsync();
    }

    public async Task<IEnumerable<Event>> GetAllByParticipantAsync(int participantId)
    {
        return await _dbContext.ParticipantEvents
            .Where(pe => pe.ParticipantId == participantId)
            .Select(pe => pe.Event!)    // !
            .ToListAsync();
    }
}
