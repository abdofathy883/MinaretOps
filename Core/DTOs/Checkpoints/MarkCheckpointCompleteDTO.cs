using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Checkpoints
{
    public class MarkCheckpointCompleteDTO
    {
        public int ClientServiceCheckpointId { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
    }
}
