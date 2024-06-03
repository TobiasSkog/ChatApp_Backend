using ChatApp.API.Models.Entities;

namespace ChatApp.API.Services;

public class RefreshTokenService(ApplicationDbContext context) : IRefreshTokenService
{
    private readonly ApplicationDbContext _context = context;
    public string GenerateRefreshToken(string username)
    {
        RefreshToken refreshToken = new()
        {
            Token = Guid.NewGuid().ToString(),
            Username = username,
            ExpiryDate = DateTime.Now.AddDays(1),
            IsRevoked = false
        };
        _context.RefreshTokens.Add(refreshToken);
        _context.SaveChanges();

        return refreshToken.Token;
    }

    public bool ValidateRefreshToken(string username, string refreshToken)
    {
        var token = _context.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken && rt.Username == username);
        return token != null && token.ExpiryDate > DateTime.Now && !token.IsRevoked;
    }

    public void RevokeRefreshToken(string refreshToken)
    {
        var token = _context.RefreshTokens.SingleOrDefault(rt => rt.Token == refreshToken);
        if (token != null)
        {
            token.IsRevoked = true;
            _context.SaveChanges();
        }
    }
}