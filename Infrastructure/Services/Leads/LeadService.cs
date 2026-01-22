using AutoMapper;
using Core.DTOs.Leads;
using Core.Interfaces;
using Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services.Leads
{
    public class LeadService : ILeadService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IEmailService emailService;
        private readonly IMapper mapper;

        public LeadService(MinaretOpsDbContext context, IEmailService emailService, IMapper mapper)
        {
            this.context = context;
            this.emailService = emailService;
            this.mapper = mapper;
        }

        public Task<LeadDTO> CreateLeadAsync(CreateLeadDTO createLeadDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteLeadAsync(Guid leadId)
        {
            throw new NotImplementedException();
        }

        public Task<List<LightWieghtLeadDTO>> GetAllLeadsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<LeadDTO> GetLeadByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<LeadDTO> UpdateLeadAsync(UpdateLeadDTO updateLeadDTO)
        {
            throw new NotImplementedException();
        }
    }
}
