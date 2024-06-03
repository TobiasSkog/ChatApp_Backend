using ChatApp.API.Models.Entities;
using ChatApp.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ChatApp.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(IConfiguration configuration, IRefreshTokenService refreshTokenService, UserManager<User> userManager, SignInManager<User> signInManager, IUserService userService) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IRefreshTokenService _refreshTokenService = refreshTokenService;
    private readonly UserManager<User> _userManager = userManager;
    private readonly SignInManager<User> _signInManager = signInManager;
    private readonly IUserService _userService = userService;

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel login)
    {
        //var existingUser = await _userManager.FindByEmailAsync(login.Username);
        var existingUser = await _userManager.FindByNameAsync(login.Username);

        if (existingUser == null)
        {
            return BadRequest();
        }
        var checkPasswordResult = await _userManager.CheckPasswordAsync(existingUser, login.Password);
        if (!checkPasswordResult)
        {
            await _userManager.AccessFailedAsync(existingUser);
            return BadRequest();
        }

        var roles = await _userManager.GetRolesAsync(existingUser);

        if (!roles.Any())
        {
            await _userManager.AddToRoleAsync(existingUser, "User");
            roles = ["User"];
        }
        var loginResponse = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.RememberMe, false);

        if (!loginResponse.Succeeded)
        {
            return Unauthorized();
        }

        var accessToken = GenerateJwtToken(login.Username);
        var refreshToken = _refreshTokenService.GenerateRefreshToken(login.Username);
        UserDto user = new()
        {
            Id = existingUser.Id,
            Username = existingUser.UserName,
            Email = existingUser.Email,
            Role = roles[0]
        };

        return Ok(new { accessToken, refreshToken, user });
    }
    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel register)
    {
        var hasher = new PasswordHasher<User>();

        User user = new()
        {
            UserName = register.Username,
            Email = register.Email,
            PasswordHash = hasher.HashPassword(null, register.Password),
            EmailConfirmed = true
        };

        var identityResult = await _userManager.CreateAsync(user, register.Password);

        if (identityResult.Succeeded)
        {
            identityResult = await _userManager.AddToRoleAsync(user, "User");
            if (identityResult.Succeeded)
            {
                return Ok("Customer account successfully created!");
            }
        }
        return BadRequest("Customer registration failed.");
    }

    [HttpPost]
    [Route("updateEmail{email}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateEmail([FromBody] string email)
    {

        var existingUser = await _userManager.FindByEmailAsync(email);
        if (existingUser == null)
        {
            return BadRequest("Customer not found.");
        }

        var result = await _userManager.ChangeEmailAsync(existingUser, email, Request.Headers.Authorization);
        if (!result.Succeeded)
        {
            return BadRequest("Error while updating Email.");
        }

        return Ok("Customer account successfully created!");
    }

    [HttpPost]
    [Route("updateUsername{username}")]
    [Authorize(Roles = "User")]
    public async Task<IActionResult> UpdateUsername([FromBody] string username)
    {
        var existingUser = await _userManager.FindByNameAsync(username);

        if (existingUser == null)
        {
            return BadRequest("Customer not found.");
        }
        existingUser.UserName = username;

        var result = await _userService.UpdateUserAsync(existingUser);
        if (result.GetType() == Results.Ok(true))
        {
            return BadRequest("Error while updating Email.");
        }

        return Ok("Customer account successfully created!");
    }

    private string GenerateJwtToken(string username)
    {
        var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, username),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        new Claim(ClaimTypes.Role, "User")
    };

        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    [HttpPost("refresh")]
    public IActionResult Refresh([FromBody] RefreshRequest request)
    {
        if (_refreshTokenService.ValidateRefreshToken(request.Username, request.RefreshToken))
        {
            var newAccessToken = GenerateJwtToken(request.Username);
            var newRefreshToken = _refreshTokenService.GenerateRefreshToken(request.Username);
            return Ok(new { accessToken = newAccessToken, refreshToken = newRefreshToken });
        }
        return Unauthorized();
    }
}
public class UserDto
{
    public int Id { get; set; }
    public string Username { get; set; }
    public string Email { get; set; }
    public string Role { get; set; }
}
public class LoginModel
{
    public string Username { get; set; }
    public string Password { get; set; }
    public bool RememberMe { get; set; }
}
public class RegisterModel
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}
public class RefreshRequest
{
    public string Username { get; set; }
    public string RefreshToken { get; set; }
}