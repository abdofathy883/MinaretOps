namespace Core.DTOs.Leads
{
    public class LeadHistoryDTO
    {
        public int Id { get; set; }
        public int SalesLeadId { get; set; }
        public string PropertyName { get; set; } = default!;
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public string? UpdatedById { get; set; }
        public string UpdatedByName { get; set; } = default!;
        public DateTime UpdatedAt { get; set; }
    }
}
