using Core.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.JDs
{
    public class JDDTO
    {
        public int Id { get; set; }
        public string RoleId { get; set; }
        public string? Role { get; set; }
        public List<JRDTO> JobResponsibilities { get; set; } = new();
    }
}
