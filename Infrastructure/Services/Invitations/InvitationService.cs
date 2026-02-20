using AutoMapper;
using Core.DTOs.EmployeeOnBoarding;
using Core.Enums;
using Core.Enums.Auth_Attendance;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services.Invitations
{
    public class InvitationService : IInvitationService
    {
        private readonly MinaretOpsDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly TaskHelperService helperService;
        private readonly IMapper mapper;

        public InvitationService(
            MinaretOpsDbContext _context,
            UserManager<ApplicationUser> _userManager,
            RoleManager<IdentityRole> _roleManager,
            TaskHelperService _helperService,
            IMapper _mapper)
        {
            context = _context;
            userManager = _userManager;
            roleManager = _roleManager;
            helperService = _helperService;
            mapper = _mapper;
        }

        public async Task<InvitationDTO> CreateInvitationAsync(CreateInvitationDTO dto, string adminUserId)
        {
            // Check if email already exists
            var existingUser = await userManager.FindByEmailAsync(dto.Email);
            if (existingUser != null)
                throw new InvalidObjectException("البريد الإلكتروني مستخدم بالفعل");

            // Check if invitation already exists for this email
            var existingInvitation = await context.EmployeeOnBoardingInvitations
                .FirstOrDefaultAsync(i => i.Email == dto.Email && i.Status == InvitationStatus.Pending);
            if (existingInvitation != null)
                throw new InvalidObjectException("تم إرسال دعوة بالفعل لهذا البريد الإلكتروني");

            using var transacrion = await context.Database.BeginTransactionAsync();
            try
            {
                // Generate secure token
                var token = GenerateSecureToken();

                var invitation = new EmployeeOnBoardingInvitation
                {
                    Email = dto.Email,
                    Role = dto.Role,
                    InvitationToken = token,
                    InvitedByUserId = adminUserId,
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddDays(7), // 7 days validity
                    Status = InvitationStatus.Pending
                };

                await context.EmployeeOnBoardingInvitations.AddAsync(invitation);

                // Send invitation email
                var invitationLink = $"https://internal.theminaretagency.com/complete-invitation?token={token}";
                var emailPayload = new
                {
                    To = dto.Email,
                    Subject = "دعوة للانضمام إلى The Minaret Agency",
                    Template = "EmployeeInvitation",
                    Replacements = new Dictionary<string, string>
                    {
                        { "{{InvitationLink}}", invitationLink },
                        { "{{RoleName}}", dto.Role.ToString() },
                        { "{{ExpiryDate}}", invitation.ExpiresAt.Value.ToString("yyyy-MM-dd") }
                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "Employee Invitation Email", emailPayload);
                await context.SaveChangesAsync();
                await transacrion.CommitAsync();

                // Notify all admins
                await NotifyAdminsAsync(invitation);

                return mapper.Map<InvitationDTO>(invitation);
            }
            catch (Exception)
            {
                await transacrion.RollbackAsync();
                throw;
            }
        }
        public async Task<InvitationDTO> GetInvitationByTokenAsync(string token)
        {
            var invitation = await context.EmployeeOnBoardingInvitations
                .Include(i => i.InvitedBy)
                .FirstOrDefaultAsync(i => i.InvitationToken == token);

            if (invitation == null)
                throw new KeyNotFoundException("الدعوة غير موجودة");

            if (invitation.Status != InvitationStatus.Pending)
                throw new InvalidObjectException("الدعوة غير صالحة");

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt.Value < DateTime.UtcNow)
            {
                invitation.Status = InvitationStatus.Expired;
                await context.SaveChangesAsync();
                throw new InvalidObjectException("انتهت صلاحية الدعوة");
            }

            return mapper.Map<InvitationDTO>(invitation);
        }
        public async Task<List<InvitationDTO>> GetPendingInvitationsAsync()
        {
            var invitations = await context.EmployeeOnBoardingInvitations
                .Include(i => i.InvitedBy)
                .Where(i => i.Status == InvitationStatus.Pending)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<InvitationDTO>>(invitations);
        }
        public async Task<InvitationDTO> CompleteInvitationAsync(CompleteInvitationDTO dto)
        {
            var invitation = await context.EmployeeOnBoardingInvitations
                .FirstOrDefaultAsync(i => i.InvitationToken == dto.Token 
                && i.Status == InvitationStatus.Pending);

            if (invitation == null)
                throw new KeyNotFoundException("الدعوة غير موجودة");

            if (invitation.ExpiresAt.HasValue && invitation.ExpiresAt.Value < DateTime.UtcNow)
            {
                invitation.Status = InvitationStatus.Expired;
                await context.SaveChangesAsync();
                throw new InvalidObjectException("انتهت صلاحية الدعوة");
            }

            // Update invitation with user's information
            invitation.FirstName = dto.FirstName;
            invitation.LastName = dto.LastName;
            invitation.PhoneNumber = dto.PhoneNumber;
            invitation.City = dto.City;
            invitation.Street = dto.Street;
            invitation.NID = dto.NID;
            invitation.PaymentNumber = dto.PaymentNumber;
            invitation.DateOfHiring = dto.DateOfHiring;
            invitation.Status = InvitationStatus.Completed;
            invitation.CompletedAt = DateTime.UtcNow;
            invitation.Password = dto.Password;

            await context.SaveChangesAsync();

            // Notify admins that invitation is completed and awaiting approval
            await NotifyAdminsForApprovalAsync(invitation);

            return mapper.Map<InvitationDTO>(invitation);
        }
        public async Task<bool> ApproveInvitationAsync(int invitationId, string adminUserId)
        {
            var invitation = await context.EmployeeOnBoardingInvitations
                .FirstOrDefaultAsync(i => i.Id == invitationId && i.Status == InvitationStatus.Completed);

            if (invitation == null)
                throw new KeyNotFoundException("الدعوة غير موجودة أو غير مكتملة");

            // Create user account
            var user = new ApplicationUser
            {
                FirstName = invitation.FirstName!,
                LastName = invitation.LastName!,
                UserName = invitation.Email.Split("@")[0],
                Email = invitation.Email,
                EmailConfirmed = true,
                PhoneNumber = invitation.PhoneNumber,
                PhoneNumberConfirmed = true,
                City = invitation.City!,
                Street = invitation.Street!,
                NID = invitation.NID!,
                PaymentNumber = invitation.PaymentNumber!,
                DateOfHiring = invitation.DateOfHiring!.Value
            };

            var result = await userManager.CreateAsync(user, invitation.Password);
            if (!result.Succeeded)
                throw new InvalidObjectException("فشل في إنشاء حساب المستخدم");

            await userManager.AddToRoleAsync(user, invitation.Role.ToString());

            // Send welcome email with temporary password
            var emailPayload = new
            {
                To = user.Email,
                Subject = "Welcome to The Minaret Agency",
                Template = "EmployeeOnBoarding",
                Replacements = new Dictionary<string, string>
                {
                    { "EmpName", $"{user.FirstName} {user.LastName}" },
                    { "EmpEmail", user.Email ?? "" },
                    { "EmpRole", invitation.Role.ToString() },
                }
            };
            await helperService.AddOutboxAsync(OutboxTypes.Email, "Employee Onboarding Email", emailPayload);
            await context.SaveChangesAsync();

            return true;
        }
        public async Task<bool> CancelInvitationAsync(int invitationId)
        {
            var invitation = await context.EmployeeOnBoardingInvitations.FindAsync(invitationId);
            if (invitation == null)
                throw new KeyNotFoundException("الدعوة غير موجودة");

            invitation.Status = InvitationStatus.Cancelled;
            await context.SaveChangesAsync();
            return true;
        }
        private string GenerateSecureToken()
        {
            var bytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(bytes);
            return Convert.ToBase64String(bytes).Replace("+", "-").Replace("/", "_").Replace("=", "");
        }
        private async Task NotifyAdminsAsync(EmployeeOnBoardingInvitation invitation)
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                var payload = new
                {
                    To = admin.Email,
                    Subject = "New Employee Has Been Invited",
                    Template = "NewEmployeeInvited",
                    Replacements = new Dictionary<string, string>
                    {

                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "New Invitation Email", payload);
                await context.SaveChangesAsync();
            }
        }
        private async Task NotifyAdminsForApprovalAsync(EmployeeOnBoardingInvitation invitation)
        {
            var admins = await userManager.GetUsersInRoleAsync("Admin");
            foreach (var admin in admins)
            {
                var payload = new
                {
                    To = admin.Email,
                    Subject = "New Employee Has Been Approved",
                    Template = "EmployeeInvitationApproved",
                    Replacements = new Dictionary<string, string>
                    {

                    }
                };
                await helperService.AddOutboxAsync(OutboxTypes.Email, "New Invitation Email", payload);
                await context.SaveChangesAsync();
            }
        }

        public async Task<List<InvitationDTO>> GetAllInvitations()
        {
            var invitations = await context.EmployeeOnBoardingInvitations
                .Include(i => i.InvitedBy)
                .OrderByDescending(i => i.CreatedAt)
                .ToListAsync();

            return mapper.Map<List<InvitationDTO>>(invitations);
        }
    }
}