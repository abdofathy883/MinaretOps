using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.DTOs.AuthDTOs
{
    public class TeamMemberDTO
    {
        public required string FullName { get; set; }
        public required string JobTitle { get; set; }
        public required string Bio { get; set; }
        public required string ProfilePicture { get; set; }
    }
}
