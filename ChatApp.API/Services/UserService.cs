using ChatApp.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.API.Services;
public class UserService(ApplicationDbContext context) : IUserService
{
    private readonly ApplicationDbContext _context = context;
    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
    public async Task<User> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }
    public async Task<User> GetUserByUsernameAsync(string username)
    {
        return await _context.Users.FindAsync(username);
    }
    public async Task<IActionResult> AddUserAsync(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
        catch
        {
            return new BadRequestResult();
        }
    }

    public async Task<IActionResult> UpdateUserAsync(User user)
    {
        try
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
        catch
        {
            return new BadRequestResult();
        }
    }
    public async Task<IActionResult> DeleteUserAsync(int id)
    {

        try
        {
            var user = await GetUserByIdAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return new OkResult();
        }
        catch
        {
            return new BadRequestResult();
        }
    }
}
