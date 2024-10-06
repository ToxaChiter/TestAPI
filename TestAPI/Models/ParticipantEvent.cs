namespace TestAPI.Models;

public class ParticipantEvent
{
    public int EventId { get; set; }
    public virtual Event? Event { get; set; }

    public int ParticipantId { get; set; }
    public virtual Participant? Participant { get; set; }

    public DateTime RegistrationDateTime { get; set; }
}
