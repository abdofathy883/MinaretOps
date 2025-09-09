using Core.Enums;
using Microsoft.AspNetCore.Http;

namespace Core.DTOs.KPI
{
    public class CreateIncedintDTO
    {
        public string EmployeeId { get; set; }
        public KPIAspectType Aspect { get; set; }
        public string? Description { get; set; }
        public IFormFile? EvidenceURL { get; set; }
    }
}
