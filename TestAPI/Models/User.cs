namespace TestAPI.Models;

public enum Role
{
    Participant,
    Admin,
}


public class User
{
    public int Id { get; set; }
    public required string Login { get; set; }
    public required string Password { get; set; }
    public required Role Role { get; set; }

    public int ParticipantId { get; set; }
    public virtual Participant? Participant { get; set; }
}
