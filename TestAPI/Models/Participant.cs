namespace TestAPI.Models;

public class Participant
{
    public int Id { get; set; }
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}"; // is needed?
    public DateOnly? DateOfBirth { get; set; }
    public required string Email { get; set; }

    public virtual List<Event> Events { get; set; } = [];
    public virtual List<ParticipantEvent> ParticipantEvents { get; set; } = [];

    public required string UserId { get; set; }
    public virtual User? User { get; set; }
}
