namespace TestAPI.Models;

public class Event
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required DateTime EventDateTime { get; set; }
    public required string Location { get; set; }
    public string? Category { get; set; }
    public int? MaxParticipants { get; set; }
    public string? PicturePath { get; set; }

    public virtual List<Participant> Participants { get; set; } = [];
    public virtual List<ParticipantEvent> ParticipantEvents { get; set; } = [];
}
