using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Checkpoints
{
    public class UpdateServiceCheckpointDTO
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int? Order { get; set; }
    }
}
