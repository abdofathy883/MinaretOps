using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.AuthDTOs
{
    public class RegisterUserDTO
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Password { get; set; }
        public required UserRoles Role { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string NID { get; set; }
        public required string PaymentNumber { get; set; }
        public required DateOnly DateOfHiring { get; set; }
        public EmployeeType EmployeeType { get; set; }
        public decimal BaseSalary { get; set; }
    }
}
