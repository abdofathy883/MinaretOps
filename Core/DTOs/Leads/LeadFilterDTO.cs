namespace Core.DTOs.Leads
{
    public class LeadFilterDTO
    {
        public string CurrentUserId { get; set; } = string.Empty;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 30;
    }
}
