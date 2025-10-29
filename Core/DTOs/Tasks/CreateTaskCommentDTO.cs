using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class CreateTaskCommentDTO
    {
        public required string Comment { get; set; }
        public required string EmployeeId { get; set; }
        public int TaskId { get; set; }
    }
}
