using Core.DTOs.Leads;

namespace Core.Interfaces.Leads
{
    public interface IleadFileService
    {
        Task<LeadImportResultDto> ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId);
        Task<byte[]> ExportLeadsToExcelAsync(string userId);
        Task<byte[]> GenerateImportTemplateAsync();
    }
}
