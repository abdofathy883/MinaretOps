namespace Core.DTOs.InternalTasks
{
    public class CreateInternalTaskDTO
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Deadline { get; set; }
        public string Priority { get; set; }
        public List<CreateInternalTaskAssignmentDTO> Assignments { get; set; }
    }
}
