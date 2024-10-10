
using Microsoft.EntityFrameworkCore;
using TestAPI.Database;
using TestAPI.Models;

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

    public async Task<bool> RegisterParticipantForEventAsync(int participantId, int eventId)
    {
        var @event = await Events.GetByIdAsync(eventId);
        if (@event == null)
        {
            return false;
        }

        var participant = await Participants.GetByIdAsync(participantId);
        if (participant == null)
        {
            return false;
        }

        var count = _dbContext.ParticipantEvents.Where(pe => pe.EventId == eventId).Count();

        if (count >= @event.MaxParticipants)
        {
            return false;
        }

        var participantEvent = new ParticipantEvent() 
        { 
            EventId = eventId, 
            ParticipantId = participantId, 
            RegistrationDateTime = DateTime.UtcNow,
        };

        _dbContext.ParticipantEvents.Add(participantEvent);

        participant.Events.Add(@event);
        participant.ParticipantEvents.Add(participantEvent);

        @event.Participants.Add(participant);
        @event.ParticipantEvents.Add(participantEvent);

        var written = await CompleteAsync();

        return written > 0;
    }

    public async Task<bool> CancelParticipantFromEventAsync(int participantId, int eventId)
    {
        var @event = await Events.GetByIdAsync(eventId);
        if (@event == null)
        {
            return false;
        }

        var participant = await Participants.GetByIdAsync(participantId);
        if (participant == null)
        {
            return false;
        }

        var participantEvent = await _dbContext.ParticipantEvents.FirstOrDefaultAsync(pe => pe.EventId == eventId && pe.ParticipantId == participantId);
        if (participantEvent == null)
        {
            return false;
        }

        _dbContext.ParticipantEvents.Remove(participantEvent);

        participant.Events.Remove(@event);
        participant.ParticipantEvents.Remove(participantEvent);

        @event.Participants.Remove(participant);
        @event.ParticipantEvents.Remove(participantEvent);

        var written = await CompleteAsync();

        return written > 0;
    }

    public async Task<DateTime?> GetRegistrationTimeAsync(int participantId, int eventId)
    {
        var @event = await Events.GetByIdAsync(eventId);
        if (@event == null)
        {
            return null;
        }

        var participant = await Participants.GetByIdAsync(participantId);
        if (participant == null)
        {
            return null;
        }

        var participantEvent = await _dbContext.ParticipantEvents.FirstOrDefaultAsync(pe => pe.EventId == eventId && pe.ParticipantId == participantId);
        if (participantEvent == null)
        {
            return null;
        }

        return participantEvent.RegistrationDateTime;
    }
}
