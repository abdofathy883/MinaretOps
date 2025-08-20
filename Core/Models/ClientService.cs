using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class ClientService
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public Client Client { get; set; } = default!;

        public int ServiceId { get; set; }
        public Service Service { get; set; } = default!;

        //public DateTime StartDate { get; set; } = DateTime.UtcNow;
        //public DateTime? EndDate { get; set; }

        // Changed from direct TaskItems to TaskGroups for month-based organization
        public List<TaskGroup> TaskGroups { get; set; } = new();

        // Helper property to get all tasks across all groups
        public List<TaskItem> GetAllTasks() => TaskGroups.SelectMany(tg => tg.Tasks).ToList();
    }
}
