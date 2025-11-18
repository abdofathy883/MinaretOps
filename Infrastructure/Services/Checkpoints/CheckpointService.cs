using AutoMapper;
using Core.DTOs.Checkpoints;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Checkpoints
{
    public class CheckpointService : ICheckpointService
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<CheckpointService> logger;

        public CheckpointService(
            MinaretOpsDbContext _dbContext,
            IMapper _mapper,
            ILogger<CheckpointService> _logger)
        {
            dbContext = _dbContext;
            mapper = _mapper;
            logger = _logger;
        }

        public async Task<ServiceCheckpointDTO> AddServiceCheckpointAsync(CreateServiceCheckpointDTO dto)
        {
            var service = await dbContext.Services
                .FirstOrDefaultAsync(s => s.Id == dto.ServiceId)
                ?? throw new InvalidObjectException("الخدمة غير موجودة");

            var checkpoint = new ServiceCheckpoint
            {
                ServiceId = dto.ServiceId,
                Name = dto.Name,
                Description = dto.Description,
                Order = dto.Order,
                CreatedAt = DateTime.UtcNow
            };

            dbContext.ServiceCheckpoints.Add(checkpoint);
            await dbContext.SaveChangesAsync();

            return mapper.Map<ServiceCheckpointDTO>(checkpoint);
        }

        public async Task<List<ServiceCheckpointDTO>> GetServiceCheckpointsAsync(int serviceId)
        {
            var checkpoints = await dbContext.ServiceCheckpoints
                .Where(sc => sc.ServiceId == serviceId)
                .OrderBy(sc => sc.Order)
                .ToListAsync();

            return mapper.Map<List<ServiceCheckpointDTO>>(checkpoints);
        }

        public async Task<ServiceCheckpointDTO> UpdateServiceCheckpointAsync(int checkpointId, UpdateServiceCheckpointDTO dto)
        {
            var checkpoint = await dbContext.ServiceCheckpoints
                .FirstOrDefaultAsync(sc => sc.Id == checkpointId)
                ?? throw new InvalidObjectException("نقطة التحقق غير موجودة");

            if (dto.Name != null) checkpoint.Name = dto.Name;
            if (dto.Description != null) checkpoint.Description = dto.Description;
            if (dto.Order.HasValue) checkpoint.Order = dto.Order.Value;

            await dbContext.SaveChangesAsync();
            return mapper.Map<ServiceCheckpointDTO>(checkpoint);
        }

        public async Task<bool> DeleteServiceCheckpointAsync(int checkpointId)
        {
            var checkpoint = await dbContext.ServiceCheckpoints
                .FirstOrDefaultAsync(sc => sc.Id == checkpointId)
                ?? throw new InvalidObjectException("نقطة التحقق غير موجودة");

            dbContext.ServiceCheckpoints.Remove(checkpoint);
            return await dbContext.SaveChangesAsync() > 0;
        }

        public async Task<List<ClientServiceCheckpointDTO>> GetClientServiceCheckpointsAsync(int clientServiceId)
        {
            var checkpoints = await dbContext.ClientServiceCheckpoints
                .Include(csc => csc.ServiceCheckpoint)
                .Include(csc => csc.CompletedByEmployee)
                .Where(csc => csc.ClientServiceId == clientServiceId)
                .OrderBy(csc => csc.ServiceCheckpoint.Order)
                .ToListAsync();

            return checkpoints.Select(csc => new ClientServiceCheckpointDTO
            {
                Id = csc.Id,
                ClientServiceId = csc.ClientServiceId,
                ServiceCheckpointId = csc.ServiceCheckpointId,
                ServiceCheckpointName = csc.ServiceCheckpoint.Name,
                ServiceCheckpointDescription = csc.ServiceCheckpoint.Description,
                ServiceCheckpointOrder = csc.ServiceCheckpoint.Order,
                IsCompleted = csc.IsCompleted,
                CompletedAt = csc.CompletedAt,
                CompletedByEmployeeId = csc.CompletedByEmployeeId,
                CompletedByEmployeeName = csc.CompletedByEmployee != null
                    ? $"{csc.CompletedByEmployee.FirstName} {csc.CompletedByEmployee.LastName}"
                    : null,
                CreatedAt = csc.CreatedAt
            }).ToList();
        }

        public async Task<ClientServiceCheckpointDTO> MarkCheckpointCompleteAsync(int clientServiceCheckpointId, string employeeId)
        {
            var checkpoint = await dbContext.ClientServiceCheckpoints
                .Include(csc => csc.ServiceCheckpoint)
                .Include(csc => csc.CompletedByEmployee)
                .FirstOrDefaultAsync(csc => csc.Id == clientServiceCheckpointId)
                ?? throw new InvalidObjectException("نقطة التحقق غير موجودة");

            var employee = await dbContext.ApplicationUsers
                .FirstOrDefaultAsync(u => u.Id == employeeId)
                ?? throw new InvalidObjectException("الموظف غير موجود");

            checkpoint.IsCompleted = true;
            checkpoint.CompletedAt = DateTime.UtcNow;
            checkpoint.CompletedByEmployeeId = employeeId;

            await dbContext.SaveChangesAsync();

            return new ClientServiceCheckpointDTO
            {
                Id = checkpoint.Id,
                ClientServiceId = checkpoint.ClientServiceId,
                ServiceCheckpointId = checkpoint.ServiceCheckpointId,
                ServiceCheckpointName = checkpoint.ServiceCheckpoint.Name,
                ServiceCheckpointDescription = checkpoint.ServiceCheckpoint.Description,
                ServiceCheckpointOrder = checkpoint.ServiceCheckpoint.Order,
                IsCompleted = checkpoint.IsCompleted,
                CompletedAt = checkpoint.CompletedAt,
                CompletedByEmployeeId = checkpoint.CompletedByEmployeeId,
                CompletedByEmployeeName = $"{employee.FirstName} {employee.LastName}",
                CreatedAt = checkpoint.CreatedAt
            };
        }

        public async Task<ClientServiceCheckpointDTO> MarkCheckpointIncompleteAsync(int clientServiceCheckpointId)
        {
            var checkpoint = await dbContext.ClientServiceCheckpoints
                .Include(csc => csc.ServiceCheckpoint)
                .Include(csc => csc.CompletedByEmployee)
                .FirstOrDefaultAsync(csc => csc.Id == clientServiceCheckpointId)
                ?? throw new InvalidObjectException("نقطة التحقق غير موجودة");

            checkpoint.IsCompleted = false;
            checkpoint.CompletedAt = null;
            checkpoint.CompletedByEmployeeId = null;

            await dbContext.SaveChangesAsync();

            return new ClientServiceCheckpointDTO
            {
                Id = checkpoint.Id,
                ClientServiceId = checkpoint.ClientServiceId,
                ServiceCheckpointId = checkpoint.ServiceCheckpointId,
                ServiceCheckpointName = checkpoint.ServiceCheckpoint.Name,
                ServiceCheckpointDescription = checkpoint.ServiceCheckpoint.Description,
                ServiceCheckpointOrder = checkpoint.ServiceCheckpoint.Order,
                IsCompleted = checkpoint.IsCompleted,
                CompletedAt = checkpoint.CompletedAt,
                CompletedByEmployeeId = checkpoint.CompletedByEmployeeId,
                CompletedByEmployeeName = null,
                CreatedAt = checkpoint.CreatedAt
            };
        }

        public async Task InitializeClientServiceCheckpointsAsync(int clientServiceId, int serviceId)
        {
            var serviceCheckpoints = await dbContext.ServiceCheckpoints
                .Where(sc => sc.ServiceId == serviceId)
                .ToListAsync();

            var existingCheckpoints = await dbContext.ClientServiceCheckpoints
                .Where(csc => csc.ClientServiceId == clientServiceId)
                .Select(csc => csc.ServiceCheckpointId)
                .ToListAsync();

            foreach (var serviceCheckpoint in serviceCheckpoints)
            {
                if (!existingCheckpoints.Contains(serviceCheckpoint.Id))
                {
                    var clientServiceCheckpoint = new ClientServiceCheckpoint
                    {
                        ClientServiceId = clientServiceId,
                        ServiceCheckpointId = serviceCheckpoint.Id,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.ClientServiceCheckpoints.Add(clientServiceCheckpoint);
                }
            }

            //await dbContext.SaveChangesAsync();
        }

        public async Task<List<ClientServiceCheckpointDTO>> CreateClientServiceCheckpointsAsync(int clientServiceId, List<int> serviceCheckpointIds)
        {
            var serviceCheckpoints = await dbContext.ServiceCheckpoints
                .Where(sc => serviceCheckpointIds.Contains(sc.Id))
                .ToListAsync();

            var existingCheckpoints = await dbContext.ClientServiceCheckpoints
                .Where(csc => csc.ClientServiceId == clientServiceId)
                .Select(csc => csc.ServiceCheckpointId)
                .ToListAsync();

            var createdCheckpoints = new List<ClientServiceCheckpoint>();

            foreach (var serviceCheckpoint in serviceCheckpoints)
            {
                if (!existingCheckpoints.Contains(serviceCheckpoint.Id))
                {
                    var clientServiceCheckpoint = new ClientServiceCheckpoint
                    {
                        ClientServiceId = clientServiceId,
                        ServiceCheckpointId = serviceCheckpoint.Id,
                        IsCompleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    dbContext.ClientServiceCheckpoints.Add(clientServiceCheckpoint);
                    createdCheckpoints.Add(clientServiceCheckpoint);
                }
            }

            await dbContext.SaveChangesAsync();

            // Return DTOs
            var checkpointIds = createdCheckpoints.Select(c => c.Id).ToList();
            return await dbContext.ClientServiceCheckpoints
                .Include(csc => csc.ServiceCheckpoint)
                .Include(csc => csc.CompletedByEmployee)
                .Where(csc => checkpointIds.Contains(csc.Id))
                .Select(csc => new ClientServiceCheckpointDTO
                {
                    Id = csc.Id,
                    ClientServiceId = csc.ClientServiceId,
                    ServiceCheckpointId = csc.ServiceCheckpointId,
                    ServiceCheckpointName = csc.ServiceCheckpoint.Name,
                    ServiceCheckpointDescription = csc.ServiceCheckpoint.Description,
                    ServiceCheckpointOrder = csc.ServiceCheckpoint.Order,
                    IsCompleted = csc.IsCompleted,
                    CompletedAt = csc.CompletedAt,
                    CompletedByEmployeeId = csc.CompletedByEmployeeId,
                    CompletedByEmployeeName = csc.CompletedByEmployee != null
                        ? $"{csc.CompletedByEmployee.FirstName} {csc.CompletedByEmployee.LastName}"
                        : null,
                    CreatedAt = csc.CreatedAt
                })
                .ToListAsync();
        }
    }
}