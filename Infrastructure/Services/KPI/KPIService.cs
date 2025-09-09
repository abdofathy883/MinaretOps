using AutoMapper;
using Core.DTOs.KPI;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.MediaUploads;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.KPI
{
    public class KPIService: IKPIService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly MediaUploadService mediaUploadService;
        private readonly IMapper mapper;
        public KPIService(
            MinaretOpsDbContext minaret,
            IEmailService email,
            MediaUploadService uploadService,
            IMapper _mapper
            )
        {
            context = minaret;
            emailService = email;
            mediaUploadService = uploadService;
            mapper = _mapper;
        }

        public async Task<IncedintDTO> NewKPIIncedintAsync(CreateIncedintDTO dto)
        {
            if (dto is null)
                throw new InvalidObjectException("Invalid data");

            var employee = await context.Users.FindAsync(dto.EmployeeId)
                ?? throw new InvalidObjectException("Employee not found");

            using var transaction = await context.Database.BeginTransactionAsync();
            try
            {
                var evidenceURL = string.Empty;
                if(dto.EvidenceURL is not null)
                {
                    var uploaded = await mediaUploadService.UploadImageWithPath(
                        dto.EvidenceURL,
                        $"{employee.FirstName} {employee.LastName} - {dto.Aspect} {DateTime.UtcNow}"
                        );
                    evidenceURL = uploaded.Url;
                }
                var incedint = new KPIIncedint
                {
                    EmployeeId = dto.EmployeeId,
                    Aspect = dto.Aspect,
                    Description = dto.Description,
                    EvidenceURL = evidenceURL,
                    TimeStamp = DateTime.UtcNow
                };
                //await context.KPIIncedints.AddAsync(incedint);
                await context.SaveChangesAsync();

                Dictionary<string, string> replacements = new Dictionary<string, string>
                {
                    { "{{EmployeeName}}", $"{employee.FirstName} {employee.LastName}" },
                    { "{{Aspect}}", dto.Aspect.ToString() },
                    { "{{Description}}", dto.Description ?? "N/A" },
                    { "{{TimeStamp}}", incedint.TimeStamp.ToString("f") },
                    { "{{PenaltyPercentage}}", incedint.PenaltyPercentage.ToString() },
                    { "{{EvidenceURL}}", incedint.EvidenceURL ?? "N/A" }
                };

                await emailService.SendEmailWithTemplateAsync(employee.Email ?? string.Empty, "incedint", "KPIIncedint", replacements);
                await transaction.CommitAsync();
                return mapper.Map<IncedintDTO>(incedint);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Failed to create incedint: {ex.Message}" );
            }
        }
    }
}
