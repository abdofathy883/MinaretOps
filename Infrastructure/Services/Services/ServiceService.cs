using AutoMapper;
using Core.DTOs.Services;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services.Services
{
    public class ServiceService : IServiceService
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly IMapper mapper;
        public ServiceService(
            MinaretOpsDbContext _dbContext,
            IMapper _mapper
            )
        {
            dbContext = _dbContext;
            mapper = _mapper;
        }
        public async Task<ServiceDTO> AddServiceAsync(CreateServiceDTO newService)
        {
            if (newService is null)
                throw new InvalidObjectException("New service cannot be null");

            var existingService = await dbContext.Services
                .AnyAsync(s => s.Title == newService.Title);

            if (existingService)
                throw new InvalidObjectException("this service already exists");

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var service = new Service
                {
                    Title = newService.Title,
                    Description = newService.Description ?? string.Empty,    
                };

                dbContext.Services.Add(service);
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();
                return mapper.Map<ServiceDTO>(service);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                throw new InvalidObjectException($"Error adding service: {ex.Message}");
            }
        }

        public async Task<bool> DeleteServiceAsync(int serviceId)
        {
            var service = await GetServiceOrThrow(serviceId);

            dbContext.Services.Remove(service);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<ServiceDTO>> GetAllServicesAsync()
        {
            var services = await dbContext.Services.ToListAsync()
                ?? throw new InvalidObjectException("No services found");

            return mapper.Map<List<ServiceDTO>>(services);
        }

        public async Task<ServiceDTO> GetServiceByIdAsync(int serviceId)
        {
            var service = await GetServiceOrThrow(serviceId);

            return mapper.Map<ServiceDTO>(service);
        }

        public async Task<ServiceDTO> ToggleVisibilityAsync(int serviceId)
        {
            var service = await GetServiceOrThrow(serviceId);

            service.IsDeleted = !service.IsDeleted;
            dbContext.Services.Update(service);
            await dbContext.SaveChangesAsync();
            return mapper.Map<ServiceDTO>(service);
        }

        public async Task<ServiceDTO> UpdateServiceAsync(UpdateServiceDTO serviceDTO)
        {
            var service = await GetServiceOrThrow(serviceDTO.Id);

            if (!string.IsNullOrWhiteSpace(serviceDTO.Title))
                service.Title = serviceDTO.Title.Trim();
            if (!string.IsNullOrWhiteSpace(serviceDTO.Description))
                service.Description = serviceDTO.Description.Trim();

            dbContext.Update(service);
            await dbContext.SaveChangesAsync();
            return mapper.Map<ServiceDTO>(service);
        }

        private async Task<Service> GetServiceOrThrow(int serviceId)
        {
            var service = await dbContext.Services
                .Include(s => s.ClientServices)
                .FirstOrDefaultAsync(s => s.Id == serviceId)
                 ?? throw new InvalidObjectException("Service not found");
            return service;
        }
    }
}
