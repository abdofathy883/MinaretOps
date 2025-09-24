using Microsoft.AspNetCore.Http;

namespace Core.DTOs.AuthDTOs
{
    public class UserDTO
    {
        public required string Id { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public List<string>? Roles { get; set; }
        //public required string ConcurrencyStamp { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string NID { get; set; }
        public required string PaymentNumber { get; set; }
        public required DateOnly DateOfHiring { get; set; }
        public required string ProfilePicture { get; set; }
        public required string JobTitle { get; set; }
        public required string Bio { get; set; }
    }
}
