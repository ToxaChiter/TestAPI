using Core.Entities;
using Core.Exceptions;
using Core.Interfaces;

namespace Application.UseCases.ParticipantUC;

public class RegisterParticipantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public RegisterParticipantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int participantId, int eventId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        if (eventEntity.Participants.Count >= eventEntity.MaxParticipants)
        {
            throw new BadRequestException("Event is full.");
        }

        var participant = await _unitOfWork.Participants.GetByIdAsync(participantId);
        if (participant is null)
        {
            throw new NotFoundException("Participant not found.");
        }

        var participantEventDb = await _unitOfWork.ParticipantEvents.GetByParticipantAndEventAsync(participantId, eventId);
        if (participantEventDb is not null)
        {
            throw new AlreadyExistsException("Participant is already registered for the event.");
        }

        var participantEvent = new ParticipantEvent()
        {
            EventId = eventId,
            ParticipantId = participantId,
            RegistrationDateTime = DateTime.UtcNow,
        };

        await _unitOfWork.ParticipantEvents.CreateAsync(participantEvent);

        participant.Events.Add(eventEntity);
        participant.ParticipantEvents.Add(participantEvent);

        eventEntity.Participants.Add(participant);
        eventEntity.ParticipantEvents.Add(participantEvent);

        await _unitOfWork.CompleteAsync();
    }
}
