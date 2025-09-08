using Core.DTOs.Complaints;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface IComplaintService
    {
        Task<List<ComplaintDTO>> GetAllComplaintsAsync();
        Task<ComplaintDTO> CreateComplaintAsync(CreateComplaintDTO complaintDTO);
    }
}
