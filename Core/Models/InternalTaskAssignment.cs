using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class InternalTaskAssignment
    {
        public int Id { get; set; }
        public int InternalTaskId { get; set; }
        public virtual InternalTask Task { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
        public bool IsLeader { get; set; }
    }
}
