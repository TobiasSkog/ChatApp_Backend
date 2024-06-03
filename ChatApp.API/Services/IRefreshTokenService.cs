namespace ChatApp.API.Services;

public interface IRefreshTokenService
{
    string GenerateRefreshToken(string username);
    bool ValidateRefreshToken(string username, string refreshToken);
    void RevokeRefreshToken(string refreshToken);
}
