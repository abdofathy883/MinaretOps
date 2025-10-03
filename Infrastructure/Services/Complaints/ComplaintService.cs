using AutoMapper;
using Core.DTOs.Complaints;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Complaints
{
    public class ComplaintService : IComplaintService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        public ComplaintService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            UserManager<ApplicationUser> user,
            IMapper _mapper
            )
        {
            context = minaret;
            emailService = email;
            userManager = user;
            mapper = _mapper;
        }
        public async Task<ComplaintDTO> CreateComplaintAsync(CreateComplaintDTO complaintDTO)
        {
            var existingComplaint = await context.Complaints
                .FirstOrDefaultAsync(c => c.Subject == complaintDTO.Subject && c.Content == complaintDTO.Content);

            if (existingComplaint is not null)
                throw new AlreadyExistObjectException("شكوى او مقترح بهذا العنوان والرسالة موجودين بالفعل");

            var emp = await userManager.FindByIdAsync(complaintDTO.EmployeeId)
                ?? throw new InvalidObjectException("لم يتم العثور على هذا الموظف");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var complaint = new Complaint
                {
                    Subject = complaintDTO.Subject,
                    Content = complaintDTO.Content,
                    EmployeeId = complaintDTO.EmployeeId,
                    User = emp,
                    CreatedAt = DateTime.UtcNow
                };
                await context.Complaints.AddAsync(complaint);
                await context.SaveChangesAsync();
                if (!string.IsNullOrEmpty(emp.Email))
                {
                    Dictionary<string, string> replacements = new Dictionary<string, string>
                    {
                        { "Subject", complaintDTO.Subject },
                        { "SubmissionMessage", complaintDTO.Content },
                        { "SubmittedBy", $"{emp.FirstName} {emp.LastName}" },
                        { "TimeStamp", complaint.CreatedAt.ToString() }
                    };
                    await emailService.SendEmailWithTemplateAsync(emp.Email, "New Complaint Received", "Complaint", replacements);
                }
                await transaction.CommitAsync();
                return mapper.Map<ComplaintDTO>(complaint);
            }
            catch(Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception(ex.Message);
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