using Core.DTOs.JDs;
using Microsoft.AspNetCore.Identity;

namespace Core.Interfaces
{
    public interface IJobDescribtionService
    {
        Task<JDDTO> CreateJDAsync(CreateJDDTO jdDTO);
        Task<List<JDDTO>> GetAllJDsAsync();
        Task<JDDTO> GetJDById(int jdId);
        Task<List<IdentityRole>> GetAllRolesAsync();
    }
}
