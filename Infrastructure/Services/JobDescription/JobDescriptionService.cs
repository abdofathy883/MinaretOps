using AutoMapper;
using Core.DTOs.JDs;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.JobDescription
{
    public class JobDescriptionService : IJobDescribtionService
    {
        private readonly MinaretOpsDbContext context;
        private readonly IMapper mapper;

        public JobDescriptionService(MinaretOpsDbContext context, IMapper _mapper)
        {
            this.context = context;
            mapper = _mapper;
        }

        public async Task<JDDTO> CreateJDAsync(CreateJDDTO jdDTO)
        {
            var jd = new Core.Models.JobDescription
            {
                RoleId = jdDTO.RoleId
            };
            await context.JobDescriptions.AddAsync(jd);
            await context.SaveChangesAsync();

            foreach (var item in jdDTO.JobResponsibilities)
            {
                var jr = new JobResponsibility
                {
                    JobDescriptionId = jd.Id,
                    Text = item.Text
                };
                await context.JobResponsibilities.AddAsync(jr);
                await context.SaveChangesAsync();
            }
            return mapper.Map<JDDTO>(jd);
        }

        public async Task<List<JDDTO>> GetAllJDsAsync()
        {
            var jds = await context.JobDescriptions
                .Include(jd => jd.JobResponsibilities)
                .ToListAsync();
            return mapper.Map<List<JDDTO>>(jds);
        }

        public async Task<JDDTO> GetJDById(int jdId)
        {
            var jd = await context.JobDescriptions
                .Include(jd => jd.JobResponsibilities)
                .FirstOrDefaultAsync(jd => jd.Id == jdId);
            return mapper.Map<JDDTO>(jd);
        }

        public async Task<List<IdentityRole>> GetAllRolesAsync()
        {
            var roles = await context.Roles.ToListAsync();
            return roles;
        }
    }
}
