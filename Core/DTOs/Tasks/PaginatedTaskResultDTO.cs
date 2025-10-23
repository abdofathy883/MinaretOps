using System.Collections.Generic;

namespace Core.DTOs.Tasks
{
    public class PaginatedTaskResultDTO
    {
        public List<TaskDTO> Records { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
