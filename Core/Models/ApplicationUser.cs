using Core.Enums;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Core.Models
{
    public class ApplicationUser: IdentityUser, IAuditable, IDeletable
    {
        [Required] [MaxLength(30)] [MinLength(3)]
        public required string FirstName { get; set; }
        [Required] [MaxLength(30)] [MinLength(3)]
        public required string LastName { get; set; }
        [Required] [MaxLength(30)] [MinLength(3)]
        public required string City { get; set; }
        [Required] [MaxLength(200)] [MinLength(3)]
        public required string Street { get; set; }
        [Required] [MaxLength(14)] [MinLength(14)]
        public required string NID { get; set; }
        [Required]
        public required string PaymentNumber { get; set; }
        [Required]
        public DateOnly DateOfHiring { get; set; }
        public List<RefreshToken> RefreshTokens { get; set; } = new();
        public List<TaskItem> TaskItems { get; set; } = new();
        public virtual List<InternalTaskAssignment> InternalTaskAssignments { get; set; } = new();
        public List<LeaveRequest> LeaveRequests { get; set; } = new();
        public List<AttendanceRecord> AttendanceRecords { get; set; } = new();
        public List<KPIIncedint> KPIIncedints { get; set; } = new();
        public List<Complaint> Complaints { get; set; } = new();
        public EmployeeType EmployeeType { get; set; }
        public decimal? BaseSalary { get; set; }

        public List<SalaryPayment> SalaryPayments { get; set; } = new();
        public List<SalaryPeriod> SalaryPeriods { get; set; } = new();
        public List<Client> Clients { get; set; } = new();

        public DateOnly CreatedAt { get; set; } = DateOnly.FromDateTime(DateTime.Today);
        public DateOnly? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
