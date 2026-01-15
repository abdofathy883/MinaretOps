using Core.DTOs.Vault;

namespace Core.DTOs.Branch
{
    public class BranchDTO
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Code { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public VaultDTO? Vault { get; set; }
    }
}
