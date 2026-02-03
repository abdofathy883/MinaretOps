namespace Core.DTOs.EmployeeOnBoarding
{
    public class CompleteInvitationDTO
    {
        public required string Token { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string City { get; set; }
        public required string Street { get; set; }
        public required string NID { get; set; }
        public required string PaymentNumber { get; set; }
        public required DateOnly DateOfHiring { get; set; }
        public required string Password { get; set; }
        public string? ProfilePicture { get; set; }
        public string? JobTitle { get; set; }
        public string? Bio { get; set; }
    }
}
