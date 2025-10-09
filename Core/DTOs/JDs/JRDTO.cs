using Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.JDs
{
    public class JRDTO
    {
        public int Id { get; set; }
        public int JobDescriptionId { get; set; }
        public required string Text { get; set; }
    }
}
