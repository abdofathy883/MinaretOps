using Core.DTOs.JDs;

namespace Core.Interfaces
{
    public interface IJobDescribtionService
    {
        Task<JDDTO> CreateJDAsync(CreateJDDTO jdDTO);
        Task<List<JDDTO>> GetAllJDsAsync();
        Task<JDDTO> GetJDById(int jdId);
    }
}
