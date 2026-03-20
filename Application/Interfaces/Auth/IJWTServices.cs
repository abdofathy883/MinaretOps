using Core.Models;

namespace Application.Interfaces.Auth
{
    public interface IJWTServices
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser appUser);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}
