using AutoMapper;
using Core.DTOs.Leads.Notes;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Services.Leads
{
    public class LeadNoteService : ILeadNoteService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public LeadNoteService(MinaretOpsDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<LeadNoteDTO> CreateNote(CreateLeadNoteDTO leadNote, string currentUserId)
        {
            var lead = await context.SalesLeads
                .SingleAsync(l => l.Id == leadNote.LeadId);

            var user = await context.Users
                .SingleAsync(u => u.Id == currentUserId);

            var note = new LeadNote
            {
                Note = leadNote.Note,
                Lead = lead,
                CreatedById = user.Id,
                CreatedAt = DateTime.UtcNow
            };

            await context.LeadNotes.AddAsync(note);
            await context.SaveChangesAsync();
            return mapper.Map<LeadNoteDTO>(note);
        }

        public async Task<List<LeadNoteDTO>> GetAllNotes(int leadId)
        {
            var notes = await context.LeadNotes
                .Where(n => n.LeadId == leadId)
                .ToListAsync();

            return mapper.Map<List<LeadNoteDTO>>(notes);
        }
    }
}
