using AutoMapper;
using Core.DTOs.Clients;
using Core.DTOs.Payloads;
using Core.Enums;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Infrastructure.Services.Checkpoints;
using Infrastructure.Services.Discord;
using Infrastructure.Services.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Clients
{
    public class ClientService : IClientServices
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly TaskHelperService taskHelperService;
        private readonly IMapper mapper;
        private readonly ILogger<ClientService> logger;
        private readonly ICheckpointService checkpointService;
        private readonly DiscordService discordService;
        public ClientService(
            MinaretOpsDbContext minaret,
            TaskHelperService _taskHelperService,
            IMapper _mapper,
            ILogger<ClientService> _logger,
            ICheckpointService _checkpointService,
            DiscordService _discordService
            )
        {
            dbContext = minaret;
            mapper = _mapper;
            logger = _logger;
            taskHelperService = _taskHelperService;
            checkpointService = _checkpointService;
            discordService = _discordService;
        }
        public async Task<List<LightWieghtClientDTO>> GetAllClientsAsync()
        {
            var clients = await dbContext.Clients
                .Include(c => c.ClientServices)
                .Select(c => new LightWieghtClientDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyName = c.CompanyName,
                    Status = c.Status,
                    ServiceId = c.ClientServices
                    .Select(cs => cs.ServiceId).FirstOrDefault(),
                    ServiceTitle = c.ClientServices
                    .Select(cs => cs.Service.Title).FirstOrDefault() ?? string.Empty
                }).ToListAsync();

            return clients;
        }
        public async Task<ClientDTO> GetClientByIdAsync(int clientId)
        {
            var client = await GetClientOrThrow(clientId);
            return mapper.Map<ClientDTO>(client);
        }
        public async Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO, string userId)
        {
            var user = await taskHelperService.GetUserOrThrow(userId)
                ?? throw new InvalidObjectException("المستخدم غير موجود");

            var existingClient = await dbContext.Clients
                .AnyAsync(c => c.Name == clientDTO.Name);

            if (existingClient)
                throw new AlreadyExistObjectException("لا يمكن اضافة عميل موجود بالفعل");

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                string? discordChannelId = null;
                try
                {
                    discordChannelId = await discordService.CreateChannelForClient(clientDTO.Name);
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to create Discord channel for client {clientDTO.Name}: {ex.Message}");
                    // Continue without Discord channel if creation fails
                }

                var newClient = new Client
                {
                    Name = clientDTO.Name,
                    CompanyName = clientDTO.CompanyName,
                    PersonalPhoneNumber = clientDTO.PersonalPhoneNumber,
                    CompanyNumber = clientDTO.CompanyNumber,
                    BusinessDescription = clientDTO.BusinessDescription,
                    DriveLink = clientDTO.DriveLink,
                    DiscordChannelId = discordChannelId ?? string.Empty,
                    ClientServices = new List<Core.Models.ClientService>()
                };
                await dbContext.Clients.AddAsync(newClient);

                foreach (var csDto in clientDTO.ClientServices)
                {
                    var clientService = new Core.Models.ClientService
                    {
                        Client = newClient,
                        ServiceId = csDto.ServiceId,
                        TaskGroups = new List<TaskGroup>()
                    };

                    await dbContext.ClientServices.AddAsync(clientService);
                    await dbContext.SaveChangesAsync();

                    if (csDto.SelectedCheckpointIds != null && csDto.SelectedCheckpointIds.Any())
                    {
                        await checkpointService.CreateClientServiceCheckpointsAsync(
                            clientService.Id,
                            csDto.SelectedCheckpointIds
                        );
                    }

                    //await checkpointService.InitializeClientServiceCheckpointsAsync(clientService.Id, csDto.ServiceId);

                    foreach (var tgDto in csDto.TaskGroups)
                    {
                        var taskGroup = new TaskGroup
                        {
                            ClientService = clientService,
                            Month = DateTime.Now.Month,
                            Year = DateTime.Now.Year,
                            MonthLabel = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}",
                            Tasks = new List<TaskItem>()
                        };

                        await dbContext.TaskGroups.AddAsync(taskGroup);

                        foreach (var taskDto in tgDto.Tasks)
                        {
                            ApplicationUser? emp = null;
                            var normalizedEmployeeId = string.IsNullOrWhiteSpace(taskDto.EmployeeId)
                                ? null : taskDto.EmployeeId;

                            if (normalizedEmployeeId is not null)
                            {
                                emp = await taskHelperService.GetUserOrThrow(normalizedEmployeeId);
                            }

                            var task = new TaskItem
                            {
                                Title = taskDto.Title,
                                TaskType = taskDto.TaskType,
                                Description = taskDto.Description,
                                Deadline = taskDto.Deadline,
                                Priority = taskDto.Priority,
                                Refrence = taskDto.Refrence,
                                EmployeeId = normalizedEmployeeId,
                                Employee = emp,
                                TaskGroup = taskGroup,
                                ClientService = clientService,
                                NumberOfSubTasks = taskDto.NumberOfSubTasks
                            };
                            await dbContext.Tasks.AddAsync(task);

                            var taskHistory = new TaskItemHistory
                            {
                                TaskItem = task,
                                PropertyName = "انشاء التاسك",
                                UpdatedById = user.Id,
                                UpdatedByName = $"{user.FirstName} {user.LastName}",
                                UpdatedAt = DateTime.UtcNow
                            };
                            await dbContext.TaskHistory.AddAsync(taskHistory);

                            // Get employee information from the database to avoid null reference
                            if (emp is not null && !string.IsNullOrEmpty(emp.Email))
                            {
                                var emailPayload = new
                                {
                                    To = emp.Email,
                                    Subject = "New Task Has Been Assigned To You",
                                    Template = "NewTaskAssignment",
                                    Replacements = new Dictionary<string, string>
                                    {
                                        {"FullName", $"{emp.FirstName} {emp.LastName}" },
                                        {"Email", $"{emp.Email}" },
                                        {"TaskTitle", $"{task.Title}" },
                                        {"TaskType", $"task.TaskType" },
                                        {"TaskId", $"{task.Id}" },
                                        {"ClientName", $"{newClient.Name}" },
                                        {"TimeStamp", $"{DateTime.UtcNow}" }
                                    }
                                };
                                await taskHelperService.AddOutboxAsync(OutboxTypes.Email, "Send New Task Email", emailPayload);
                                string? channelId = newClient?.DiscordChannelId;
                                if (channelId != null)
                                {
                                    var discordPayload = new DiscordPayload(channelId, task, DiscordOperationType.NewTask);
                                    await taskHelperService.AddOutboxAsync(OutboxTypes.Discord, "Send New Discord Notification", discordPayload);
                                }
                            }
                        }
                    }
                }

                // Final save to persist all tasks
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                // Reload the client with all related entities for mapping
                var clientForMapping = await dbContext.Clients
                    .Include(c => c.ClientServices)
                        .ThenInclude(cs => cs.Service)
                    .Include(c => c.ClientServices)
                        .ThenInclude(cs => cs.TaskGroups)
                            .ThenInclude(tg => tg.Tasks)
                                .ThenInclude(t => t.Employee)
                    .Include(c => c.ClientServices)
                        .ThenInclude(cs => cs.ClientServiceCheckpoints)
                            .ThenInclude(csc => csc.ServiceCheckpoint)
                    .Include(c => c.ClientServices)
                        .ThenInclude(cs => cs.ClientServiceCheckpoints)
                            .ThenInclude(csc => csc.CompletedByEmployee)
                    .FirstOrDefaultAsync(c => c.Id == newClient.Id);

                return mapper.Map<ClientDTO>(clientForMapping ?? newClient);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                logger.LogError($"Error adding new client: {ex}");
                throw;
            }
        }
        public async Task<bool> DeleteClientAsync(int clientId)
        {
            var client = await GetClientOrThrow(clientId);
            dbContext.Remove(client);
            return await dbContext.SaveChangesAsync() > 0;
        }
        public async Task<ClientDTO> UpdateClientAsync(int clientId, UpdateClientDTO updateClientDTO)
        {
            if (clientId == 0 || updateClientDTO is null)
                throw new InvalidObjectException("لا يوجد عميل بهذه البيانات");

            var client = await dbContext.Clients.FirstOrDefaultAsync(c => c.Id == clientId)
                ?? throw new InvalidObjectException("لا يوجد عميل بهذه البيانات");

            client.Name = updateClientDTO.Name ?? client.Name;
            client.PersonalPhoneNumber = updateClientDTO.PersonalPhoneNumber ?? client.PersonalPhoneNumber;
            client.CompanyName = updateClientDTO.CompanyName ?? client.CompanyName;
            client.CompanyNumber = updateClientDTO.CompanyNumber ?? client.CompanyNumber;
            client.BusinessDescription = updateClientDTO.BusinessDescription ?? client.BusinessDescription;
            client.DriveLink = updateClientDTO.DriveLink ?? client.DriveLink;
            client.DiscordChannelId = updateClientDTO.DiscordChannelId;
            client.Status = updateClientDTO.Status;
            client.StatusNotes = updateClientDTO.StatusNotes ?? client.StatusNotes;

            dbContext.Update(client);
            await dbContext.SaveChangesAsync();
            return mapper.Map<ClientDTO>(client);
        }
        private async Task<Client> GetClientOrThrow(int clientId)
        {
            var client = await dbContext.Clients
            .Include(c => c.ClientServices)
                .ThenInclude(cs => cs.Service)
            .Include(c => c.ClientServices)
                .ThenInclude(cs => cs.TaskGroups)
                    .ThenInclude(tg => tg.Tasks)
                        .ThenInclude(t => t.Employee)
            .FirstOrDefaultAsync(c => c.Id == clientId)
                ?? throw new InvalidObjectException("لا يوجد عميل بهذه البيانات");

            return client;
        }
    }
}
