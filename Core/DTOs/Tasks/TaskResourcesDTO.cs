using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.Tasks
{
    public class TaskResourcesDTO
    {
        public int Id { get; set; }
        public int TaskId { get; set; }
        public required string URL { get; set; }
    }
}
