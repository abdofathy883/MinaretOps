using Core.Enums;

namespace Core.Models
{
    public class KPIIncedint
    {
        public int Id { get; set; }
        public string? EmployeeId { get; set; }
        public KPIAspectType Aspect { get; set; }
        public DateTime TimeStamp { get; set; }
        public int PenaltyPercentage { get; set; } = 10; // currently fixed
        public string? Description { get; set; }
        public string? EvidenceURL { get; set; }
        public DateOnly? Date { get; set; }

        public ApplicationUser Employee { get; set; } = default!;
    }
}
