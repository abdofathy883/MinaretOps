using Core.Enums.Auth_Attendance;
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
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string NID { get; set; }
        public required string PaymentNumber { get; set; }
        public required DateOnly DateOfHiring { get; set; }
        public decimal BaseSalary { get; set; }
        public EmployeeType EmployeeType { get; set; }
    }
}
