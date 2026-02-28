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
            var rows = worksheet.RowsUsed().Skip(1).ToList(); // Skip header

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
                    existingLead.Country = GetStringOrNull(worksheet.Cell(rowNumber, 3));
                    existingLead.Occupation = GetStringOrNull(worksheet.Cell(rowNumber, 4));
                    existingLead.ContactStatus = ParseEnum<ContactStatus>(worksheet.Cell(rowNumber, 5).GetValue<string>());
                    existingLead.CurrentLeadStatus = ParseEnum<CurrentLeadStatus>(worksheet.Cell(rowNumber, 6).GetValue<string>());
                    existingLead.LeadSource = ParseEnum<LeadSource>(worksheet.Cell(rowNumber, 7).GetValue<string>());
                    existingLead.FreelancePlatform = ParseNullableEnum<FreelancePlatform>(worksheet.Cell(rowNumber, 8).GetValue<string>());
                    existingLead.Responsibility = ParseEnum<LeadResponsibility>(worksheet.Cell(rowNumber, 9).GetValue<string>());
                    existingLead.Budget = ParseEnum<LeadBudget>(worksheet.Cell(rowNumber, 10).GetValue<string>());
                    existingLead.Timeline = ParseEnum<LeadTimeline>(worksheet.Cell(rowNumber, 11).GetValue<string>());
                    existingLead.NeedsExpectation = ParseEnum<NeedsExpectation>(worksheet.Cell(rowNumber, 12).GetValue<string>());
                    existingLead.InterestLevel = ParseEnum<InterestLevel>(worksheet.Cell(rowNumber, 13).GetValue<string>());
                    existingLead.MeetingDate = worksheet.Cell(rowNumber, 14).GetValue<DateTime?>();
                    existingLead.FollowUpTime = worksheet.Cell(rowNumber, 15).GetValue<DateTime?>();
                    existingLead.QuotationSent = worksheet.Cell(rowNumber, 16).GetValue<bool>();

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
                        Country = GetStringOrNull(worksheet.Cell(rowNumber, 3)),
                        Occupation = GetStringOrNull(worksheet.Cell(rowNumber, 4)),
                        ContactStatus = ParseEnum<ContactStatus>(worksheet.Cell(rowNumber, 5).GetValue<string>()),
                        CurrentLeadStatus = ParseEnum<CurrentLeadStatus>(worksheet.Cell(rowNumber, 6).GetValue<string>()),
                        LeadSource = ParseEnum<LeadSource>(worksheet.Cell(rowNumber, 7).GetValue<string>()),
                        FreelancePlatform = ParseNullableEnum<FreelancePlatform>(worksheet.Cell(rowNumber, 8).GetValue<string>()),
                        Responsibility = ParseEnum<LeadResponsibility>(worksheet.Cell(rowNumber, 9).GetValue<string>()),
                        Budget = ParseEnum<LeadBudget>(worksheet.Cell(rowNumber, 10).GetValue<string>()),
                        Timeline = ParseEnum<LeadTimeline>(worksheet.Cell(rowNumber, 11).GetValue<string>()),
                        NeedsExpectation = ParseEnum<NeedsExpectation>(worksheet.Cell(rowNumber, 12).GetValue<string>()),
                        InterestLevel = ParseEnum<InterestLevel>(worksheet.Cell(rowNumber, 13).GetValue<string>()),
                        MeetingDate = worksheet.Cell(rowNumber, 14).GetValue<DateTime?>(),
                        FollowUpTime = worksheet.Cell(rowNumber, 15).GetValue<DateTime?>(),
                        QuotationSent = worksheet.Cell(rowNumber, 16).GetValue<bool>(),
                        CreatedById = currentUserId,
                        SalesRepId = currentUserId,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.SalesLeads.Add(newLead);
                }
            }
            await context.SaveChangesAsync();

            // Second pass: update ServicesInterestedIn and Notes by WhatsApp (we have Ids now)
            foreach (var row in rows)
            {
                var rowNumber = row.RowNumber();
                var whatsapp = worksheet.Cell(rowNumber, 2).GetValue<string>();
                if (string.IsNullOrWhiteSpace(whatsapp)) continue;

                var lead = await context.SalesLeads
                    .Include(l => l.ServicesInterestedIn)
                    .Include(l => l.Notes)
                    .FirstOrDefaultAsync(l => l.WhatsAppNumber == whatsapp);
                if (lead == null) continue;

                var servicesCell = worksheet.Cell(rowNumber, 17).GetValue<string>();
                if (!string.IsNullOrWhiteSpace(servicesCell))
                {
                    var titles = servicesCell.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                    var services = await context.Services
                        .Where(s => titles.Contains(s.Title))
                        .ToListAsync();
                    context.LeadServices.RemoveRange(lead.ServicesInterestedIn);
                    foreach (var service in services)
                    {
                        context.LeadServices.Add(new LeadServices { LeadId = lead.Id, ServiceId = service.Id });
                    }
                }

                var notesCell = worksheet.Cell(rowNumber, 18).GetValue<string>();
                if (!string.IsNullOrWhiteSpace(notesCell))
                {
                    var hasNote = lead.Notes.Any(n => n.Note == notesCell);
                    if (!hasNote)
                    {
                        lead.Notes.Add(new LeadNote
                        {
                            Note = notesCell,
                            CreatedById = currentUserId,
                            LeadId = lead.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
            await context.SaveChangesAsync();
        }
        private static T? ParseNullableEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            return Enum.TryParse<T>(value, true, out var result) ? result : null;
        }

        private static T ParseEnum<T>(string value) where T : struct
        {
            if (string.IsNullOrWhiteSpace(value)) return default;
            return Enum.TryParse<T>(value, true, out var result) ? result : default;
        }

        private static string? GetStringOrNull(IXLCell cell)
        {
            var v = cell.GetValue<string>();
            return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
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
                .Include(x => x.Notes)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Leads");

            // Headers (column order must match import)
            worksheet.Cell(1, 1).Value = "Business Name";
            worksheet.Cell(1, 2).Value = "WhatsApp Number";
            worksheet.Cell(1, 3).Value = "Country";
            worksheet.Cell(1, 4).Value = "Occupation";
            worksheet.Cell(1, 5).Value = "Contact Status";
            worksheet.Cell(1, 6).Value = "Current Lead Status";
            worksheet.Cell(1, 7).Value = "Lead Source";
            worksheet.Cell(1, 8).Value = "Freelance Platform";
            worksheet.Cell(1, 9).Value = "Responsibility";
            worksheet.Cell(1, 10).Value = "Budget";
            worksheet.Cell(1, 11).Value = "Timeline";
            worksheet.Cell(1, 12).Value = "Needs Expectation";
            worksheet.Cell(1, 13).Value = "Interest Level";
            worksheet.Cell(1, 14).Value = "Meeting Date";
            worksheet.Cell(1, 15).Value = "Follow Up Time";
            worksheet.Cell(1, 16).Value = "Quotation Sent";
            worksheet.Cell(1, 17).Value = "Services Interested In";
            worksheet.Cell(1, 18).Value = "Notes";

            int row = 2;
            foreach (var lead in leads)
            {
                worksheet.Cell(row, 1).Value = lead.BusinessName;
                worksheet.Cell(row, 2).Value = lead.WhatsAppNumber;
                worksheet.Cell(row, 3).Value = lead.Country ?? "";
                worksheet.Cell(row, 4).Value = lead.Occupation ?? "";
                worksheet.Cell(row, 5).Value = lead.ContactStatus.ToString();
                worksheet.Cell(row, 6).Value = lead.CurrentLeadStatus.ToString();
                worksheet.Cell(row, 7).Value = lead.LeadSource.ToString();
                worksheet.Cell(row, 8).Value = lead.FreelancePlatform?.ToString() ?? "";
                worksheet.Cell(row, 9).Value = lead.Responsibility.ToString();
                worksheet.Cell(row, 10).Value = lead.Budget.ToString();
                worksheet.Cell(row, 11).Value = lead.Timeline.ToString();
                worksheet.Cell(row, 12).Value = lead.NeedsExpectation.ToString();
                worksheet.Cell(row, 13).Value = lead.InterestLevel.ToString();
                worksheet.Cell(row, 14).Value = lead.MeetingDate?.ToString() ?? "";
                worksheet.Cell(row, 15).Value = lead.FollowUpTime?.ToString() ?? "";
                worksheet.Cell(row, 16).Value = lead.QuotationSent;
                worksheet.Cell(row, 17).Value = string.Join(", ", lead.ServicesInterestedIn.Where(s => s.Service != null).Select(s => s.Service!.Title));
                worksheet.Cell(row, 18).Value = string.Join("; ", lead.Notes.Where(n => !string.IsNullOrEmpty(n.Note)).Select(n => n.Note!));
                row++;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}
