using Microsoft.AspNetCore.Identity;

namespace ChatApp.API.Models.Entities;
public class User : IdentityUser<int>
{
    public ICollection<Message> Messages { get; set; } = [];
    public ICollection<UserRoom> UserRooms { get; set; } = [];
}
