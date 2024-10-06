namespace TestAPI.DTOs;

public class ParticipantDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime DateOfBirth { get; set; }
    public DateTime DateOfRegistration { get; set; }
    public string Email { get; set; }
}
