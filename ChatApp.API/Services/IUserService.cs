using ChatApp.API.Models.Entities;
using Microsoft.AspNetCore.Mvc;
namespace ChatApp.API.Services;
public interface IUserService
{
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<User> GetUserByIdAsync(int id);
    Task<IActionResult> AddUserAsync(User user);
    Task<IActionResult> UpdateUserAsync(User user);
    Task<IActionResult> DeleteUserAsync(int id);
}
