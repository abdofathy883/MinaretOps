using System.Text.Json.Serialization;

namespace Core.DTOs.AuthDTOs
{
    public class AuthResponseDTO
    {
        public string? Id { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? Street { get; set; }
        public string? NID { get; set; }
        public string? PaymentNumber { get; set; }
        public DateOnly DateOfHiring { get; set; }
        public string? Message { get; set; }
        public bool IsAuthenticated { get; set; }
        public bool IsDeleted { get; set; }
        public string? UserName { get; set; }
        public List<string>? Roles { get; set; }
        public string? Token { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public string? ConcurrencyStamp { get; set; }
    }
}
