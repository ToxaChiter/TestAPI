using Microsoft.AspNetCore.Identity;

namespace TestAPI.Models;

public class Participant : IdentityUser
{
    public required string FirstName { get; set; }
    public string? LastName { get; set; }
    public string FullName => $"{FirstName} {LastName}"; // is needed?
    public DateTime? DateOfBirth { get; set; }

    public virtual List<Event> Events { get; set; } = [];
    public virtual List<ParticipantEvent> ParticipantEvents { get; set; } = [];
}
