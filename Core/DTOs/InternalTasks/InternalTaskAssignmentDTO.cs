namespace Core.DTOs.InternalTasks
{
    public class InternalTaskAssignmentDTO
    {
        public int Id { get; set; }
        public int InternalTaskId { get; set; }
        public string UserId { get; set; }
        public bool IsLeader { get; set; }
    }
}
