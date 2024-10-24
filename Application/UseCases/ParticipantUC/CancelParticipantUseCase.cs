using Core.Exceptions;
using Core.Interfaces;

namespace Application.UseCases.ParticipantUC;

public class CancelParticipantUseCase
{
    private readonly IUnitOfWork _unitOfWork;

    public CancelParticipantUseCase(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task ExecuteAsync(int eventId, int participantId)
    {
        var eventEntity = await _unitOfWork.Events.GetByIdAsync(eventId);
        if (eventEntity is null)
        {
            throw new NotFoundException("Event not found.");
        }

        var participant = await _unitOfWork.Participants.GetByIdAsync(participantId);
        if (participant is null)
        {
            throw new NotFoundException("Participant not found.");
        }

        var participantEvent = await _unitOfWork.ParticipantEvents.GetByParticipantAndEventAsync(participantId, eventId);
        if (participantEvent is null)
        {
            throw new BadRequestException("Participant isn't registered for the event.");
        }

        eventEntity.Participants.Remove(participant);
        await _unitOfWork.Events.UpdateAsync(eventEntity);
    }
}
