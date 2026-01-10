using Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Vault
    {
        public int Id { get; set; }
        public VaultType VaultType { get; set; }
        public int? BranchId { get; set; }
        public Branch? Branch { get; set; }
    }
}
