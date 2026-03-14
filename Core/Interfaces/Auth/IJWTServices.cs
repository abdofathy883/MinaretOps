using Core.Models;

namespace Core.Interfaces.Auth
{
    public interface IJWTServices
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser appUser);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}
