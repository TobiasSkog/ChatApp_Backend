namespace ChatApp.API.Models.Entities;
public class Message
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public User User { get; set; }
    public int RoomId { get; set; }
    public Room Room { get; set; }
    public string Content { get; set; }
    public DateTime TimeStamp { get; set; }
}
