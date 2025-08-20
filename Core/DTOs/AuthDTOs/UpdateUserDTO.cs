namespace Core.DTOs.AuthDTOs
{
    public class UpdateUserDTO
    {
        public required string Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? PaymentNumber { get; set; }
    }
}
