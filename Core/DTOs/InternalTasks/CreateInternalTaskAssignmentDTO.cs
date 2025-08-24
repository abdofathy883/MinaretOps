using Core.Models;

namespace Core.DTOs.InternalTasks
{
    public class CreateInternalTaskAssignmentDTO
    {
        public int InternalTaskId { get; set; }
        public string UserId { get; set; }
        public bool IsLeader { get; set; }
    }
}
