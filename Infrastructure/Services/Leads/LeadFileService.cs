using ClosedXML.Excel;
using Core.DTOs.Leads;
using Core.Enums.Leads;
using Core.Helpers;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Infrastructure.Services.Leads
{
    public class LeadFileService : IleadFileService
    {
        private readonly MinaretOpsDbContext context;

        public LeadFileService(MinaretOpsDbContext context)
        {
            this.context = context;
        }
        public async Task<byte[]> GenerateImportTemplateAsync()
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("Leads");

            // Hidden reference sheet for dropdowns (bypasses Excel's 255-char inline limit)
            var refSheet = workbook.Worksheets.Add("_Ref");
            refSheet.Visibility = XLWorksheetVisibility.VeryHidden;

            string[] headers =
            {
                "Business Name", "WhatsApp Number", "Country", "Occupation",
                "Contact Status", "Current Lead Status", "Lead Source",
                "Freelance Platform", "Responsibility", "Budget",
                "Timeline", "Needs Expectation", "Interest Level",
                "Meeting Date", "Follow Up Time", "Quotation Sent",
                "Services Interested In", "Notes"
            };

            for (int i = 0; i < headers.Length; i++)
            {
                var cell = ws.Cell(1, i + 1);
                cell.Value = headers[i];
                cell.Style.Font.Bold = true;
                cell.Style.Fill.BackgroundColor = XLColor.LightGray;
            }

            int templateRows = 1000;
            int refCol = 1; // each enum gets its own column in _Ref sheet

            void AddDropdownFromRef(int wsColumn, List<string> values)
            {
                for (int i = 0; i < values.Count; i++)
                    refSheet.Cell(i + 1, refCol).Value = values[i];

                var rangeAddress = $"'_Ref'!${ToColumnLetter(refCol)}$1:${ToColumnLetter(refCol)}${values.Count}";
                var dataRange = ws.Range(2, wsColumn, templateRows + 1, wsColumn);
                dataRange.CreateDataValidation().List(rangeAddress, true);

                refCol++;
            }

            AddDropdownFromRef(5, EnumHelper.GetAllDescriptions<ContactStatus>());
            AddDropdownFromRef(6, EnumHelper.GetAllDescriptions<CurrentLeadStatus>());
            AddDropdownFromRef(7, EnumHelper.GetAllDescriptions<LeadSource>());
            AddDropdownFromRef(8, EnumHelper.GetAllDescriptions<FreelancePlatform>());
            AddDropdownFromRef(9, EnumHelper.GetAllDescriptions<LeadResponsibility>());
            AddDropdownFromRef(10, EnumHelper.GetAllDescriptions<LeadBudget>());
            AddDropdownFromRef(11, EnumHelper.GetAllDescriptions<LeadTimeline>());
            AddDropdownFromRef(12, EnumHelper.GetAllDescriptions<NeedsExpectation>());
            AddDropdownFromRef(13, EnumHelper.GetAllDescriptions<InterestLevel>());
            AddDropdownFromRef(16, new List<string> { "TRUE", "FALSE" });

            var services = await context.Services
                .Where(s => !s.IsDeleted)
                .Select(s => s.Title)
                .ToListAsync();

            if (services.Count > 0)
                AddDropdownFromRef(17, services);

            // Format date columns
            ws.Range(2, 14, templateRows + 1, 14).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";
            ws.Range(2, 15, templateRows + 1, 15).Style.DateFormat.Format = "yyyy-MM-dd HH:mm";

            // Add sample row so users know the expected format
            ws.Cell(2, 1).Value = "Example Business";
            ws.Cell(2, 2).Value = "+201001234567";
            ws.Cell(2, 3).Value = "Egypt";
            ws.Cell(2, 4).Value = "Restaurant Owner";
            ws.Cell(2, 14).Value = "";
            ws.Cell(2, 15).Value = "";

            ws.Row(2).Style.Font.Italic = true;
            ws.Row(2).Style.Font.FontColor = XLColor.Gray;

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string ToColumnLetter(int col)
        {
            string result = "";
            while (col > 0)
            {
                col--;
                result = (char)('A' + col % 26) + result;
                col /= 26;
            }
            return result;
        }

        public async Task<LeadImportResultDto> ImportLeadsFromExcelAsync(Stream fileStream, string currentUserId)
        {
            var result = new LeadImportResultDto();

            using var workbook = new XLWorkbook(fileStream);
            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed().Skip(1).ToList();

            result.TotalRows = rows.Count;

            foreach (var row in rows)
            {
                var rowNumber = row.RowNumber();
                var businessName = GetStringOrNull(worksheet.Cell(rowNumber, 1));
                var whatsapp = GetStringOrNull(worksheet.Cell(rowNumber, 2));

                if (string.IsNullOrWhiteSpace(whatsapp) || string.IsNullOrWhiteSpace(businessName))
                {
                    result.SkippedCount++;
                    if (!string.IsNullOrWhiteSpace(whatsapp) || !string.IsNullOrWhiteSpace(businessName))
                    {
                        result.Errors.Add(new LeadImportRowError
                        {
                            RowNumber = rowNumber,
                            BusinessName = businessName ?? "",
                            WhatsAppNumber = whatsapp ?? "",
                            ErrorMessage = "Both Business Name and WhatsApp Number are required."
                        });
                        result.FailedCount++;
                        result.SkippedCount--;
                    }
                    continue;
                }

                try
                {
                    var existingLead = await context.SalesLeads
                        .Include(l => l.ServicesInterestedIn)
                        .Include(l => l.Notes)
                        .FirstOrDefaultAsync(l =>
                            l.BusinessName == businessName && l.WhatsAppNumber == whatsapp);

                    if (existingLead != null)
                    {
                        UpdateLeadFromRow(existingLead, worksheet, rowNumber);
                        existingLead.UpdatedAt = DateTime.UtcNow;
                        context.SalesLeads.Update(existingLead);

                        await UpsertServicesAndNotes(existingLead, worksheet, rowNumber, currentUserId);

                        result.UpdatedCount++;
                    }
                    else
                    {
                        var newLead = CreateLeadFromRow(worksheet, rowNumber, currentUserId);
                        context.SalesLeads.Add(newLead);
                        var newHistory = CreateLeadHistoryFromRow(newLead, currentUserId);
                        await context.SaveChangesAsync();

                        await UpsertServicesAndNotes(newLead, worksheet, rowNumber, currentUserId);

                        result.CreatedCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add(new LeadImportRowError
                    {
                        RowNumber = rowNumber,
                        BusinessName = businessName,
                        WhatsAppNumber = whatsapp,
                        ErrorMessage = ex.Message
                    });
                }
            }

            await context.SaveChangesAsync();
            return result;
        }

        private void UpdateLeadFromRow(SalesLead lead, IXLWorksheet ws, int row)
        {
            lead.Country = GetStringOrNull(ws.Cell(row, 3));
            lead.Occupation = GetStringOrNull(ws.Cell(row, 4));
            lead.ContactStatus = EnumHelper.ParseFromDescription<ContactStatus>(ws.Cell(row, 5).GetValue<string>());
            lead.CurrentLeadStatus = EnumHelper.ParseFromDescription<CurrentLeadStatus>(ws.Cell(row, 6).GetValue<string>());
            lead.LeadSource = EnumHelper.ParseFromDescription<LeadSource>(ws.Cell(row, 7).GetValue<string>());
            lead.FreelancePlatform = EnumHelper.ParseNullableFromDescription<FreelancePlatform>(ws.Cell(row, 8).GetValue<string>());
            lead.Responsibility = EnumHelper.ParseFromDescription<LeadResponsibility>(ws.Cell(row, 9).GetValue<string>());
            lead.Budget = EnumHelper.ParseFromDescription<LeadBudget>(ws.Cell(row, 10).GetValue<string>());
            lead.Timeline = EnumHelper.ParseFromDescription<LeadTimeline>(ws.Cell(row, 11).GetValue<string>());
            lead.NeedsExpectation = EnumHelper.ParseFromDescription<NeedsExpectation>(ws.Cell(row, 12).GetValue<string>());
            lead.InterestLevel = EnumHelper.ParseFromDescription<InterestLevel>(ws.Cell(row, 13).GetValue<string>());
            lead.MeetingDate = GetDateTimeOrNull(ws.Cell(row, 14));
            lead.FollowUpTime = GetDateTimeOrNull(ws.Cell(row, 15));
            lead.QuotationSent = GetBoolValue(ws.Cell(row, 16));
        }

        private SalesLead CreateLeadFromRow(IXLWorksheet ws, int row, string currentUserId)
        {
            return new SalesLead
            {
                BusinessName = ws.Cell(row, 1).GetValue<string>().Trim(),
                WhatsAppNumber = ws.Cell(row, 2).GetValue<string>().Trim(),
                Country = GetStringOrNull(ws.Cell(row, 3)),
                Occupation = GetStringOrNull(ws.Cell(row, 4)),
                ContactStatus = EnumHelper.ParseFromDescription<ContactStatus>(ws.Cell(row, 5).GetValue<string>()),
                CurrentLeadStatus = EnumHelper.ParseFromDescription<CurrentLeadStatus>(ws.Cell(row, 6).GetValue<string>()),
                LeadSource = EnumHelper.ParseFromDescription<LeadSource>(ws.Cell(row, 7).GetValue<string>()),
                FreelancePlatform = EnumHelper.ParseNullableFromDescription<FreelancePlatform>(ws.Cell(row, 8).GetValue<string>()),
                Responsibility = EnumHelper.ParseFromDescription<LeadResponsibility>(ws.Cell(row, 9).GetValue<string>()),
                Budget = EnumHelper.ParseFromDescription<LeadBudget>(ws.Cell(row, 10).GetValue<string>()),
                Timeline = EnumHelper.ParseFromDescription<LeadTimeline>(ws.Cell(row, 11).GetValue<string>()),
                NeedsExpectation = EnumHelper.ParseFromDescription<NeedsExpectation>(ws.Cell(row, 12).GetValue<string>()),
                InterestLevel = EnumHelper.ParseFromDescription<InterestLevel>(ws.Cell(row, 13).GetValue<string>()),
                MeetingDate = GetDateTimeOrNull(ws.Cell(row, 14)),
                FollowUpTime = GetDateTimeOrNull(ws.Cell(row, 15)),
                QuotationSent = GetBoolValue(ws.Cell(row, 16)),
                CreatedById = currentUserId,
                SalesRepId = currentUserId,
                CreatedAt = DateTime.UtcNow
            };
        }

        private async Task<SalesLeadHistory> CreateLeadHistoryFromRow(SalesLead lead, string currentUserId)
        {
            var currentUser = await context.Users.SingleAsync(u => u.Id == currentUserId);
            return new SalesLeadHistory
            {
                SalesLead = lead,
                PropertyName = "استيراد من Excel",
                OldValue = null,
                NewValue = lead.BusinessName,
                UpdatedById = currentUser.Id,
                UpdatedByName = $"{currentUser.FirstName} {currentUser.LastName}"
            };
        }

        private async Task UpsertServicesAndNotes(SalesLead lead, IXLWorksheet ws, int row, string currentUserId)
        {
            var servicesCell = GetStringOrNull(ws.Cell(row, 17));
            if (!string.IsNullOrWhiteSpace(servicesCell))
            {
                var titles = servicesCell.Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
                var services = await context.Services
                    .Where(s => titles.Contains(s.Title) && !s.IsDeleted)
                    .ToListAsync();

                var existingServiceIds = lead.ServicesInterestedIn.Select(s => s.ServiceId).ToHashSet();
                foreach (var service in services)
                {
                    if (!existingServiceIds.Contains(service.Id))
                    {
                        context.LeadServices.Add(new LeadServices { LeadId = lead.Id, ServiceId = service.Id });
                    }
                }
            }

            var notesCell = GetStringOrNull(ws.Cell(row, 18));
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

        public async Task<byte[]> ExportLeadsToExcelAsync(string userId)
        {
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
            worksheet.Cell(1, 19).Value = "Qualification Score";

            int row = 2;
            foreach (var lead in leads)
            {
                worksheet.Cell(row, 1).Value = lead.BusinessName;
                worksheet.Cell(row, 2).Value = lead.WhatsAppNumber;
                worksheet.Cell(row, 3).Value = lead.Country ?? "";
                worksheet.Cell(row, 4).Value = lead.Occupation ?? "";
                worksheet.Cell(row, 5).Value = EnumHelper.GetDescription(lead.ContactStatus);
                worksheet.Cell(row, 6).Value = EnumHelper.GetDescription(lead.CurrentLeadStatus);
                worksheet.Cell(row, 7).Value = EnumHelper.GetDescription(lead.LeadSource);
                worksheet.Cell(row, 8).Value = lead.FreelancePlatform.HasValue
                    ? EnumHelper.GetDescription(lead.FreelancePlatform.Value)
                    : "";
                worksheet.Cell(row, 9).Value = EnumHelper.GetDescription(lead.Responsibility);
                worksheet.Cell(row, 10).Value = EnumHelper.GetDescription(lead.Budget);
                worksheet.Cell(row, 11).Value = EnumHelper.GetDescription(lead.Timeline);
                worksheet.Cell(row, 12).Value = EnumHelper.GetDescription(lead.NeedsExpectation);
                worksheet.Cell(row, 13).Value = EnumHelper.GetDescription(lead.InterestLevel);
                worksheet.Cell(row, 14).Value = lead.MeetingDate?.ToString("yyyy-MM-dd HH:mm") ?? "";
                worksheet.Cell(row, 15).Value = lead.FollowUpTime?.ToString("yyyy-MM-dd HH:mm") ?? "";
                worksheet.Cell(row, 16).Value = lead.QuotationSent;
                worksheet.Cell(row, 17).Value = string.Join(", ", lead.ServicesInterestedIn.Where(s => s.Service != null).Select(s => s.Service!.Title));
                worksheet.Cell(row, 18).Value = string.Join("; ", lead.Notes.Where(n => !string.IsNullOrEmpty(n.Note)).Select(n => n.Note!));
                worksheet.Cell(row, 19).Value = LeadQualificationCalculator.Calculate(
                    lead.Budget, lead.Responsibility, lead.InterestLevel, lead.Timeline, lead.NeedsExpectation);
                row++;
            }

            worksheet.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private static string? GetStringOrNull(IXLCell cell)
        {
            var v = cell.GetValue<string>();
            return string.IsNullOrWhiteSpace(v) ? null : v.Trim();
        }

        private static DateTime? GetDateTimeOrNull(IXLCell cell)
        {
            try
            {
                if (cell.IsEmpty()) return null;
                var str = cell.GetValue<string>();
                if (string.IsNullOrWhiteSpace(str)) return null;
                return DateTime.TryParse(str, out var dt) ? dt : null;
            }
            catch
            {
                return null;
            }
        }

        private static bool GetBoolValue(IXLCell cell)
        {
            try
            {
                if (cell.IsEmpty()) return false;
                var str = cell.GetValue<string>().Trim();
                return str.Equals("TRUE", StringComparison.OrdinalIgnoreCase)
                    || str == "1"
                    || str.Equals("yes", StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}
