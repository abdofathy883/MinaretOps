using AutoMapper;
using Core.DTOs.Complaints;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Complaints
{
    public class ComplaintService : IComplaintService
    {
        private readonly MinaretOpsDbContext context;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;
        public ComplaintService(
            MinaretOpsDbContext minaret,
            TaskHelperService _helperService,
            IMapper _mapper
            )
        {
            context = minaret;
            helperService = _helperService;
            mapper = _mapper;
        }
        public async Task<ComplaintDTO> CreateComplaintAsync(CreateComplaintDTO complaintDTO)
        {
            var existingComplaint = await context.Complaints
                .FirstOrDefaultAsync(c => c.Subject == complaintDTO.Subject && c.Content == complaintDTO.Content);

            if (existingComplaint is not null)
                throw new AlreadyExistObjectException("شكوى او مقترح بهذا العنوان والرسالة موجودين بالفعل");

            var emp = await helperService.GetUserOrThrow(complaintDTO.EmployeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var complaint = new Complaint
                {
                    Subject = complaintDTO.Subject,
                    Content = complaintDTO.Content,
                    EmployeeId = emp.Id,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Complaints.AddAsync(complaint);
                if (!string.IsNullOrEmpty(emp.Email))
                {
                    var emailPayload = new
                    {
                        To = emp.Email,
                        Subject = "New Complaint Received",
                        Template = "Complaint",
                        Replacement = new Dictionary<string, string>
                        {
                            { "Subject", complaintDTO.Subject },
                            { "SubmissionMessage", complaintDTO.Content },
                            { "SubmittedBy", $"{emp.FirstName} {emp.LastName}" },
                            { "TimeStamp", complaint.CreatedAt.ToString() }
                        }
                    };
                    await helperService.AddOutboxAsync(Core.Enums.OutboxTypes.Email, "New Complaint Email", emailPayload);
                }
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                return mapper.Map<ComplaintDTO>(complaint);
            }
            catch(Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<List<ComplaintDTO>> GetAllComplaintsAsync()
        {
            var complaints = await context.Complaints
                .Include(c => c.User).ToListAsync();
            return mapper.Map<List<ComplaintDTO>>(complaints);
        }
    }
}