namespace Core.DTOs.Services
{
    public class CreateServiceDTO
    {
        public required string Title { get; set; }
        public string? Description { get; set; }
    }
}
