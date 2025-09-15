namespace Core.DTOs.LoggingDTO
{
    public class FrontendLogDTO
    {
        public string Level { get; set; }
        public string Message { get; set; }
        public object? Data { get; set; }
    }
}
