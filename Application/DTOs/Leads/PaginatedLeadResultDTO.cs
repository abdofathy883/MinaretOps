namespace Application.DTOs.Leads
{
    public class PaginatedLeadResultDTO
    {
        public List<LeadDTO> Records { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
