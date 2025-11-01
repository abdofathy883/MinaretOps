namespace Core.DTOs.Tasks.TaskResourcesDTOs
{
    public class CreateTaskResourcesDTO
    {
        public int TaskId { get; set; }
        public required List<string> URLs { get; set; }
        public string? CompletionNotes { get; set; }
    }
}
