using ClosedXML.Excel;
using Core.Enums.Leads;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Leads
{
    public class LeadFileService : IleadFileService
    {
        private readonly MinaretOpsDbContext context;

        public LeadFileService(MinaretOpsDbContext context)
        {
            this.context = context;
        }

        public async Task ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId)
        {
            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1); // Skip header

            foreach (var row in rows)
            {
                var rowNumber = row.RowNumber();
                var whatsapp = worksheet.Cell(rowNumber, 2).GetValue<string>(); // Col 2: WhatsApp
                if (string.IsNullOrWhiteSpace(whatsapp)) continue;

                var existingLead = await context.SalesLeads
                    .Include(l => l.ServicesInterestedIn)
                    .FirstOrDefaultAsync(l => l.WhatsAppNumber == whatsapp);

                if (existingLead != null)
                {
                    // Update
                    existingLead.BusinessName = worksheet.Cell(rowNumber, 1).GetValue<string>();
                    existingLead.ContactStatus = ParseEnum<ContactStatus>(worksheet.Cell(rowNumber, 3).GetValue<string>());
                    existingLead.CurrentLeadStatus = ParseEnum<CurrentLeadStatus>(worksheet.Cell(rowNumber, 4).GetValue<string>());
                    existingLead.LeadSource = ParseEnum<LeadSource>(worksheet.Cell(rowNumber, 6).GetValue<string>());
                    existingLead.InterestLevel = ParseEnum<InterestLevel>(worksheet.Cell(rowNumber, 9).GetValue<string>());
                    existingLead.MeetingDate = worksheet.Cell(rowNumber, 11).GetValue<DateTime?>();
                    existingLead.QuotationSent = worksheet.Cell(rowNumber, 13).GetValue<bool>();
                    existingLead.FollowUpTime = worksheet.Cell(rowNumber, 14).GetValue<DateTime?>();

                    existingLead.UpdatedAt = DateTime.UtcNow;
                    context.SalesLeads.Update(existingLead);
                }
                else
                {
                    // Create
                    var newLead = new SalesLead
                    {
                        BusinessName = worksheet.Cell(rowNumber, 1).GetValue<string>(),
                        WhatsAppNumber = whatsapp,
                        ContactStatus = ParseEnum<ContactStatus>(worksheet.Cell(rowNumber, 3).GetValue<string>()),
                        CurrentLeadStatus = ParseEnum<CurrentLeadStatus>(worksheet.Cell(rowNumber, 4).GetValue<string>()),
                        LeadSource = ParseEnum<LeadSource>(worksheet.Cell(rowNumber, 6).GetValue<string>()),
                        InterestLevel = ParseEnum<InterestLevel>(worksheet.Cell(rowNumber, 9).GetValue<string>()),
                        MeetingDate = worksheet.Cell(rowNumber, 11).GetValue<DateTime?>(),
                        QuotationSent = worksheet.Cell(rowNumber, 13).GetValue<bool>(),
                        FollowUpTime = worksheet.Cell(rowNumber, 14).GetValue<DateTime?>(),
                        CreatedById = currentUserId,
                        SalesRepId = currentUserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.SalesLeads.Add(newLead);
                }
            }
            await context.SaveChangesAsync();
        }
        private T ParseEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            return Enum.TryParse<T>(value, true, out var result) ? result : default;
        }
        public async Task<byte[]> ExportLeadsToExcelAsync(string userId)
        {
            //var leads = await GetAllLeadsAsync(userId);
            var leads = await context.SalesLeads
                .AsNoTracking()
                .Where(x => x.SalesRepId == userId)
                .Include(x => x.SalesRep)
                .Include(x => x.CreatedBy)
                .Include(x => x.ServicesInterestedIn)
                    .ThenInclude(x => x.Service)
                .ToListAsync();
            // Although GetAllLeadsAsync returns DTOs, we might want entities or just map DTOs.
            // DTOs are fine.

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Leads");

            // Headers
            worksheet.Cell(1, 1).Value = "Business Name";
            worksheet.Cell(1, 2).Value = "WhatsApp Number";
            worksheet.Cell(1, 3).Value = "Employee";
            worksheet.Cell(1, 4).Value = "Contact Status";
            worksheet.Cell(1, 5).Value = "Current Lead Status";
            worksheet.Cell(1, 6).Value = "Contact Attempts";
            worksheet.Cell(1, 7).Value = "Lead Source";
            worksheet.Cell(1, 8).Value = "Decision Maker Reached";
            worksheet.Cell(1, 9).Value = "Interested";
            worksheet.Cell(1, 10).Value = "Interest Level";
            worksheet.Cell(1, 11).Value = "Meeting Agreed";
            worksheet.Cell(1, 12).Value = "Meeting Date";
            worksheet.Cell(1, 13).Value = "Meeting Attend";
            worksheet.Cell(1, 14).Value = "Quotation Sent";
            worksheet.Cell(1, 15).Value = "Follow Up Time";
            worksheet.Cell(1, 16).Value = "Follow Up Reason";
            worksheet.Cell(1, 17).Value = "Notes";

            int row = 2;
            foreach (var lead in leads)
            {
                worksheet.Cell(row, 1).Value = lead.BusinessName;
                worksheet.Cell(row, 2).Value = lead.WhatsAppNumber;
                worksheet.Cell(row, 3).Value = lead.SalesRep.FirstName ?? "NA";
                worksheet.Cell(row, 4).Value = lead.ContactStatus.ToString();
                worksheet.Cell(row, 5).Value = lead.CurrentLeadStatus.ToString();
                worksheet.Cell(row, 7).Value = lead.LeadSource.ToString();
                worksheet.Cell(row, 10).Value = lead.InterestLevel.ToString();
                worksheet.Cell(row, 12).Value = lead.MeetingDate?.ToString();
                worksheet.Cell(row, 14).Value = lead.QuotationSent;
                worksheet.Cell(row, 15).Value = lead.FollowUpTime?.ToString();
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
