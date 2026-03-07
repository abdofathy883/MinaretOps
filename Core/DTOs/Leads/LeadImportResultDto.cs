namespace Core.DTOs.Leads
{
    public class LeadImportResultDto
    {
        public int TotalRows { get; set; }
        public int SuccessCount => CreatedCount + UpdatedCount;
        public int CreatedCount { get; set; }
        public int UpdatedCount { get; set; }
        public int FailedCount { get; set; }
        public int SkippedCount { get; set; }
        public List<LeadImportRowError> Errors { get; set; } = new();
    }

    public class LeadImportRowError
    {
        public int RowNumber { get; set; }
        public string BusinessName { get; set; } = string.Empty;
        public string WhatsAppNumber { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;
    }
}
