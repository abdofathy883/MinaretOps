using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IJWTServices
    {
        Task<string> GenerateAccessTokenAsync(ApplicationUser appUser);
        Task<RefreshToken> GenerateRefreshTokenAsync();
    }
}
