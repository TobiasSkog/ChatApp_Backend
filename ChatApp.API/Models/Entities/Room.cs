namespace ChatApp.API.Models.Entities;
public class Room
{
    public int Id { get; set; }
    public string Name { get; set; }
    public ICollection<Message> Messages { get; set; }
    public ICollection<UserRoom> UserRooms { get; set; }
}
