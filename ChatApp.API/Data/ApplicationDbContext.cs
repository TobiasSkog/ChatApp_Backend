using ChatApp.API.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API;
public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<User, IdentityRole<int>, int>(options)
{
    public DbSet<Message> Messages { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<UserRoom>()
            .HasKey(ur => new { ur.UserId, ur.RoomId });

        base.OnModelCreating(modelBuilder);
    }
}




//modelBuilder.Entity<UserRoom>()
//    .HasOne(ur => ur.User)
//    .WithMany(u => u.UserRooms)
//    .HasForeignKey(ur => ur.UserId);

//modelBuilder.Entity<UserRoom>()
//    .HasOne(ur => ur.Room)
//    .WithMany(r => r.UserRooms)
//    .HasForeignKey(ur => ur.RoomId);

//modelBuilder.Entity<Room>()
//    .HasMany(r => r.Messages)
//    .WithOne(m => m.Room)
//    .HasForeignKey(m => m.RoomId);

//modelBuilder.Entity<User>()
//    .HasMany(u => u.Messages)
//    .WithOne(m => m.User)
//    .HasForeignKey(m => m.UserId);