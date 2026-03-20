using System.Collections.Generic;

namespace Application.DTOs.Tasks.TaskDTOs
{
    public class PaginatedTaskResultDTO
    {
        public List<LightWieghtTaskDTO> Records { get; set; } = new();
        public int TotalRecords { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }
}
