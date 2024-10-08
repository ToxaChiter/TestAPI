using Microsoft.AspNetCore.Identity;

namespace TestAPI.Models;

public class User : IdentityUser
{
    public new int Id { get; set; }

    //public int ParticipantId { get; set; }
    public virtual Participant? Participant { get; set; }
}
