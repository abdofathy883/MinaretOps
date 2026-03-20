using Application.DTOs.Leads;

namespace Application.Interfaces.Leads
{
    public interface IleadFileService
    {
        Task<LeadImportResultDto> ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId);
        Task<byte[]> ExportLeadsToExcelAsync(string userId);
        Task<byte[]> GenerateImportTemplateAsync();
    }
}
