namespace Core.DTOs.Leads.Notes
{
    public class LeadNoteDTO
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public required string CreatedById { get; set; }
        public required string CreatedByName { get; set; }
        public int LeadId { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
