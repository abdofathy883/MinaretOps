namespace Core.DTOs.JDs
{
    public class CreateJDDTO
    {
        public string RoleId { get; set; }
        public List<CreateJRDTO> JobResponsibilities { get; set; } = new();
    }
}
