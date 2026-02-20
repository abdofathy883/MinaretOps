namespace Core.Interfaces
{
    public interface IleadFileService
    {
        Task ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId);
        Task<byte[]> ExportLeadsToExcelAsync(string userId);
    }
}
