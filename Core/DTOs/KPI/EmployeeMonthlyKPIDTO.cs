namespace Core.DTOs.KPI
{
    public class EmployeeMonthlyKPIDTO
    {
        public string EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public string MonthLabel => new DateTime(Year, Month, 1).ToString("yyyy-MM");

        public int Commitment { get; set; }
        public int Productivity { get; set; }
        public int QualityOfWork { get; set; }
        public int Cooperation { get; set; }
        public int CustomerSatisfaction { get; set; }

        public int Total => Commitment + Productivity + QualityOfWork + Cooperation + CustomerSatisfaction;
    }
}