using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public int VaultId { get; set; }
        public Vault Vault { get; set; } = default!;
    }
}
