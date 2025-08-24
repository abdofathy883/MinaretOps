using Core.Models;

namespace Core.Interfaces
{
    public interface IJWTServices
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser appUser);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}
