namespace Core.DTOs.Reports
{
    public class MonthlyTaskGroupDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public string MonthLabel { get; set; } = string.Empty;
        public List<TaskReportDTO> Tasks { get; set; } = new();
    }
}
