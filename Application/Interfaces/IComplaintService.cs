using Application.DTOs.Complaints;

namespace Application.Interfaces
{
    public interface IComplaintService
    {
        Task<List<ComplaintDTO>> GetAllComplaintsAsync();
        Task<ComplaintDTO> CreateComplaintAsync(CreateComplaintDTO complaintDTO);
    }
}
