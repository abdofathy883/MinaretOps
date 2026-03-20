using Application.Interfaces;
using AutoMapper;
using Application.DTOs.Clients;
using Application.DTOs.ClientServiceDTOs;
using Application.DTOs.Payloads;
using Application.DTOs.Tasks.TaskDTOs;
using Application.DTOs.Tasks.TaskGroupDTOs;
using Core.Enums;
using Core.Models;
using DocumentFormat.OpenXml.InkML;
using Infrastructure.Exceptions;
using Infrastructure.Services.Checkpoints;
using Infrastructure.Services.Discord;
using Infrastructure.Services.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net;
using Infrastructure.Persistance;

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
        public async Task<List<LightWieghtClientDTO>> GetAllActiveAsync()
        {
            var clients = await dbContext.Clients
                .Where(c => c.Status == ClientStatus.Active)
                .Include(c => c.ClientServices)
                    .ThenInclude(cs => cs.Service)
                .Include(c => c.AccountManager)
                .Select(c => new LightWieghtClientDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyName = c.CompanyName,
                    Status = c.Status,
                    ServiceId = c.ClientServices
                    .Select(cs => cs.ServiceId).FirstOrDefault(),
                    ServiceTitle = c.ClientServices
                    .Select(cs => cs.Service.Title).FirstOrDefault() ?? string.Empty,
                    AccountManagerId = c.AccountManagerId,
                    AccountManagerName = c.AccountManager != null
                        ? $"{c.AccountManager.FirstName} {c.AccountManager.LastName}"
                        : string.Empty
                }).ToListAsync();

            return clients;
        }
        public async Task<List<LightWieghtClientDTO>> GetAllAsync(ClientStatus status)
        {
            var clients = await dbContext.Clients
                .Where(c => c.Status == status)
                .Include(c => c.ClientServices)
                    .ThenInclude(cs => cs.Service)
                .Include(c => c.AccountManager)
                .Select(c => new LightWieghtClientDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    CompanyName = c.CompanyName,
                    Status = c.Status,
                    ServiceId = c.ClientServices
                    .Select(cs => cs.ServiceId).FirstOrDefault(),
                    ServiceTitle = c.ClientServices
                    .Select(cs => cs.Service.Title).FirstOrDefault() ?? string.Empty,
                    AccountManagerId = c.AccountManagerId,
                    AccountManagerName = c.AccountManager != null
                        ? $"{c.AccountManager.FirstName} {c.AccountManager.LastName}"
                        : string.Empty
                }).ToListAsync();

            return clients;
        }
        public async Task<ClientDTO> GetClientByIdAsync(int clientId)
        {
            var client = await GetClientOrThrow(clientId);
            return mapper.Map<ClientDTO>(client);
        }
        //public async Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO, string userId)
        //{
        //    var user = await taskHelperService.GetUserOrThrow(userId)
        //        ?? throw new InvalidObjectException("المستخدم غير موجود");

        //    var existingClient = await dbContext.Clients
        //        .AnyAsync(c => c.Name == clientDTO.Name);

        //    if (existingClient)
        //        throw new AlreadyExistObjectException("لا يمكن اضافة عميل موجود بالفعل");

        //    using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
        //    try
        //    {
        //        string? discordChannelId = null;
        //        try
        //        {
        //            discordChannelId = await discordService.CreateChannelForClient(clientDTO.Name);
        //        }
        //        catch (Exception ex)
        //        {
        //            logger.LogWarning($"Failed to create Discord channel for client {clientDTO.Name}: {ex.Message}");
        //            // Continue without Discord channel if creation fails
        //        }

        //        var newClient = new Client
        //        {
        //            Name = clientDTO.Name,
        //            CompanyName = clientDTO.CompanyName,
        //            PersonalPhoneNumber = clientDTO.PersonalPhoneNumber,
        //            CompanyNumber = clientDTO.CompanyNumber,
        //            Email = clientDTO.Email,
        //            BusinessDescription = clientDTO.BusinessDescription,
        //            DriveLink = clientDTO.DriveLink,
        //            DiscordChannelId = discordChannelId ?? string.Empty,
        //            BusinessType = clientDTO.BusinessType,
        //            BusinessActivity = clientDTO.BusinessActivity,
        //            CommercialRegisterNumber = clientDTO.CommercialRegisterNumber,
        //            TaxCardNumber = clientDTO.TaxCardNumber,
        //            Country = clientDTO.Country,
        //            AccountManagerId = clientDTO.AccountManagerId,
        //            ClientServices = new List<Core.Models.ClientService>()
        //        };
        //        await dbContext.Clients.AddAsync(newClient);

        //        foreach (var csDto in clientDTO.ClientServices)
        //        {
        //            var clientService = new Core.Models.ClientService
        //            {
        //                Client = newClient,
        //                ServiceId = csDto.ServiceId,
        //                ServiceCost = csDto.ServiceCost,
        //                TaskGroups = new List<TaskGroup>()
        //            };

        //            await dbContext.ClientServices.AddAsync(clientService);
        //            await dbContext.SaveChangesAsync();

        //            if (csDto.SelectedCheckpointIds != null && csDto.SelectedCheckpointIds.Any())
        //            {
        //                await checkpointService.CreateClientServiceCheckpointsAsync(
        //                    clientService.Id,
        //                    csDto.SelectedCheckpointIds
        //                );
        //            }

        //            //await checkpointService.InitializeClientServiceCheckpointsAsync(clientService.Id, csDto.ServiceId);

        //            foreach (var tgDto in csDto.TaskGroups)
        //            {
        //                var taskGroup = new TaskGroup
        //                {
        //                    ClientService = clientService,
        //                    Month = DateTime.Now.Month,
        //                    Year = DateTime.Now.Year,
        //                    MonthLabel = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}",
        //                    Tasks = new List<TaskItem>()
        //                };

        //                await dbContext.TaskGroups.AddAsync(taskGroup);

        //                foreach (var taskDto in tgDto.Tasks)
        //                {
        //                    ApplicationUser? emp = null;
        //                    var normalizedEmployeeId = string.IsNullOrWhiteSpace(taskDto.EmployeeId)
        //                        ? null : taskDto.EmployeeId;

        //                    if (normalizedEmployeeId is not null)
        //                    {
        //                        emp = await taskHelperService.GetUserOrThrow(normalizedEmployeeId);
        //                    }

        //                    var task = new TaskItem
        //                    {
        //                        Title = taskDto.Title,
        //                        TaskType = taskDto.TaskType,
        //                        Description = taskDto.Description,
        //                        Deadline = taskDto.Deadline,
        //                        Priority = taskDto.Priority,
        //                        Refrence = taskDto.Refrence,
        //                        EmployeeId = normalizedEmployeeId,
        //                        Employee = emp,
        //                        TaskGroup = taskGroup,
        //                        ClientService = clientService,
        //                        NumberOfSubTasks = taskDto.NumberOfSubTasks
        //                    };
        //                    await dbContext.Tasks.AddAsync(task);

        //                    var taskHistory = new TaskItemHistory
        //                    {
        //                        TaskItem = task,
        //                        PropertyName = "انشاء التاسك",
        //                        UpdatedById = user.Id,
        //                        UpdatedByName = $"{user.FirstName} {user.LastName}",
        //                        UpdatedAt = DateTime.UtcNow
        //                    };
        //                    await dbContext.TaskHistory.AddAsync(taskHistory);

        //                    // Get employee information from the database to avoid null reference
        //                    if (emp is not null && !string.IsNullOrEmpty(emp.Email))
        //                    {
        //                        var emailPayload = new
        //                        {
        //                            To = emp.Email,
        //                            Subject = "New Task Has Been Assigned To You",
        //                            Template = "NewTaskAssignment",
        //                            Replacements = new Dictionary<string, string>
        //                            {
        //                                {"FullName", $"{emp.FirstName} {emp.LastName}" },
        //                                {"Email", $"{emp.Email}" },
        //                                {"TaskTitle", $"{task.Title}" },
        //                                {"TaskType", $"task.TaskType" },
        //                                {"TaskId", $"{task.Id}" },
        //                                {"ClientName", $"{newClient.Name}" },
        //                                {"TimeStamp", $"{DateTime.UtcNow}" }
        //                            }
        //                        };
        //                        await taskHelperService.AddOutboxAsync(OutboxTypes.Email, "Send New Task Email", emailPayload);
        //                        string? channelId = newClient?.DiscordChannelId;
        //                        if (channelId != null)
        //                        {
        //                            var discordPayload = new DiscordPayload(channelId, task, DiscordOperationType.NewTask);
        //                            await taskHelperService.AddOutboxAsync(OutboxTypes.Discord, "Send New Discord Notification", discordPayload);
        //                        }
        //                    }
        //                }
        //            }
        //        }

        //        // Final save to persist all tasks
        //        await dbContext.SaveChangesAsync();
        //        await dbTransaction.CommitAsync();

        //        // Reload the client with all related entities for mapping
        //        var clientForMapping = await dbContext.Clients
        //            .Include(c => c.AccountManager)
        //            .Include(c => c.ClientServices)
        //                .ThenInclude(cs => cs.Service)
        //            .Include(c => c.ClientServices)
        //                .ThenInclude(cs => cs.TaskGroups)
        //                    .ThenInclude(tg => tg.Tasks)
        //                        .ThenInclude(t => t.Employee)
        //            .Include(c => c.ClientServices)
        //                .ThenInclude(cs => cs.ClientServiceCheckpoints)
        //                    .ThenInclude(csc => csc.ServiceCheckpoint)
        //            .Include(c => c.ClientServices)
        //                .ThenInclude(cs => cs.ClientServiceCheckpoints)
        //                    .ThenInclude(csc => csc.CompletedByEmployee)
        //            .FirstOrDefaultAsync(c => c.Id == newClient.Id);

        //        return mapper.Map<ClientDTO>(clientForMapping ?? newClient);
        //    }
        //    catch (Exception ex)
        //    {
        //        await dbTransaction.RollbackAsync();
        //        logger.LogError($"Error adding new client: {ex}");
        //        throw;
        //    }
        //}

        public async Task<List<LightWieghtClientDTO>> SearchAsync(string clientName)
        {
            if (string.IsNullOrWhiteSpace(clientName))
                return new List<LightWieghtClientDTO>();

            clientName = clientName.Trim();

            IQueryable<Client> clientsQuery = dbContext.Clients
                .Include(c => c.ClientServices)
                    .ThenInclude(cs => cs.Service)
                .Include(c => c.AccountManager);

            // Search condition
            clientsQuery = clientsQuery.Where(t =>
                EF.Functions.Like(t.Name, $"%{clientName}%"));

            var clients = await clientsQuery
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            return mapper.Map<List<LightWieghtClientDTO>>(clients);
        }
        public async Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO, string userId)
        {
            var user = await taskHelperService.GetUserOrThrow(userId)
                ?? throw new InvalidObjectException("المستخدم غير موجود");

            await EnsureClientNotDuplicated(clientDTO.Name);

            using var tx = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var discordChannelId = await TryCreateDiscordChannel(clientDTO.Name);
                var client = BuildClient(clientDTO, discordChannelId);
                await dbContext.Clients.AddAsync(client);

                foreach (var csDto in clientDTO.ClientServices)
                {
                    await ProcessClientService(client, csDto, user);
                }

                await dbContext.SaveChangesAsync();
                await tx.CommitAsync();

                return await ReloadAndMapClient(client.Id);
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                logger.LogError("Error adding new client: {Ex}", ex);
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

            var client = await dbContext.Clients
                .Include(c => c.AccountManager)
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
                //.FirstOrDefaultAsync(c => c.Id == client.Id);
                //.Include(c => c.AccountManager)
                //.Include(c => c.ClientServices)
                .FirstOrDefaultAsync(c => c.Id == clientId)
                ?? throw new InvalidObjectException("لا يوجد عميل بهذه البيانات");

            if (client.Name != updateClientDTO.Name && !string.IsNullOrEmpty(updateClientDTO.Name))
                client.Name = updateClientDTO.Name;

            if (client.PersonalPhoneNumber != updateClientDTO.PersonalPhoneNumber 
                && !string.IsNullOrEmpty(updateClientDTO.PersonalPhoneNumber))
                client.PersonalPhoneNumber = updateClientDTO.PersonalPhoneNumber;

            if (client.CompanyName != updateClientDTO.CompanyName 
                && !string.IsNullOrEmpty(updateClientDTO.CompanyName))
                client.CompanyName = updateClientDTO.CompanyName;

            if (client.CompanyNumber != updateClientDTO.CompanyNumber 
                && !string.IsNullOrEmpty(updateClientDTO.CompanyNumber))
                client.CompanyNumber = updateClientDTO.CompanyNumber;

            if (client.Email != updateClientDTO.Email 
                && !string.IsNullOrEmpty(updateClientDTO.Email))
                client.Email = updateClientDTO.Email;

            if (client.BusinessDescription != updateClientDTO.BusinessDescription 
                && !string.IsNullOrEmpty(updateClientDTO.BusinessDescription))
                client.BusinessDescription = updateClientDTO.BusinessDescription;

            if (client.DriveLink != updateClientDTO.DriveLink 
                && !string.IsNullOrEmpty(updateClientDTO.DriveLink))
                client.DriveLink = updateClientDTO.DriveLink;

            if (client.DiscordChannelId != updateClientDTO.DiscordChannelId 
                && !string.IsNullOrEmpty(updateClientDTO.DiscordChannelId))
                client.DiscordChannelId = updateClientDTO.DiscordChannelId;

            if (client.BusinessActivity != updateClientDTO.BusinessActivity 
                && !string.IsNullOrEmpty(updateClientDTO.BusinessActivity))
                client.BusinessActivity = updateClientDTO.BusinessActivity;

            if (client.BusinessType != updateClientDTO.BusinessType)
                client.BusinessType = updateClientDTO.BusinessType;

            if (client.CommercialRegisterNumber != updateClientDTO.CommercialRegisterNumber 
                && !string.IsNullOrEmpty(updateClientDTO.CommercialRegisterNumber))
                client.CommercialRegisterNumber = updateClientDTO.CommercialRegisterNumber;

            if (client.TaxCardNumber != updateClientDTO.TaxCardNumber 
                && !string.IsNullOrEmpty(updateClientDTO.TaxCardNumber))
                client.TaxCardNumber = updateClientDTO.TaxCardNumber;

            if (client.Country != updateClientDTO.Country 
                && !string.IsNullOrEmpty(updateClientDTO.Country))
                client.Country = updateClientDTO.Country;

            if (client.AccountManagerId != updateClientDTO.AccountManagerId 
                && !string.IsNullOrEmpty(updateClientDTO.AccountManagerId))
                client.AccountManagerId = updateClientDTO.AccountManagerId;

            if (client.Status != updateClientDTO.Status)
                client.Status = updateClientDTO.Status;

            if (client.StatusNotes != updateClientDTO.StatusNotes 
                && !string.IsNullOrEmpty(updateClientDTO.StatusNotes))
                client.StatusNotes = updateClientDTO.StatusNotes;

            dbContext.Update(client);
            await dbContext.SaveChangesAsync();

            // Reload the client with all related entities for mapping
            //var clientForMapping = await dbContext.Clients
            //    .AsNoTracking()
            //    .Include(c => c.AccountManager)
            //    .Include(c => c.ClientServices)
            //        .ThenInclude(cs => cs.Service)
            //    .Include(c => c.ClientServices)
            //        .ThenInclude(cs => cs.TaskGroups)
            //            .ThenInclude(tg => tg.Tasks)
            //                .ThenInclude(t => t.Employee)
            //    .Include(c => c.ClientServices)
            //        .ThenInclude(cs => cs.ClientServiceCheckpoints)
            //            .ThenInclude(csc => csc.ServiceCheckpoint)
            //    .Include(c => c.ClientServices)
            //        .ThenInclude(cs => cs.ClientServiceCheckpoints)
            //            .ThenInclude(csc => csc.CompletedByEmployee)
            //    .FirstOrDefaultAsync(c => c.Id == client.Id);

            return mapper.Map<ClientDTO>(client);
            //return mapper.Map<ClientDTO>(client);
        }
        private async Task<Client> GetClientOrThrow(int clientId)
        {
            var client = await dbContext.Clients
                .Include(c => c.AccountManager)
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


        private async Task EnsureClientNotDuplicated(string name)
        {
            var exists = await dbContext.Clients.AnyAsync(c => c.Name == name);
            if (exists) throw new AlreadyExistObjectException("لا يمكن اضافة عميل موجود بالفعل");
        }

        private async Task<string> TryCreateDiscordChannel(string clientName)
        {
            try
            {
                return await discordService.CreateChannelForClient(clientName) ?? string.Empty;
            }
            catch (Exception ex)
            {
                logger.LogWarning("Failed to create Discord channel for {Client}: {Msg}", clientName, ex.Message);
                return string.Empty;
            }
        }

        private Client BuildClient(CreateClientDTO dto, string discordChannelId) => new()
        {
            Name = dto.Name,
            CompanyName = dto.CompanyName,
            PersonalPhoneNumber = dto.PersonalPhoneNumber,
            CompanyNumber = dto.CompanyNumber,
            Email = dto.Email,
            BusinessDescription = dto.BusinessDescription,
            DriveLink = dto.DriveLink,
            DiscordChannelId = discordChannelId,
            BusinessType = dto.BusinessType,
            BusinessActivity = dto.BusinessActivity,
            CommercialRegisterNumber = dto.CommercialRegisterNumber,
            TaxCardNumber = dto.TaxCardNumber,
            Country = dto.Country,
            AccountManagerId = dto.AccountManagerId,
            ClientServices = new List<Core.Models.ClientService>()
        };

        private async Task ProcessClientService(Client client, CreateClientServiceDTO csDto, ApplicationUser user)
        {
            var clientService = new Core.Models.ClientService
            {
                Client = client,
                ServiceId = csDto.ServiceId,
                ServiceCost = csDto.ServiceCost,
                TaskGroups = new List<TaskGroup>()
            };

            await dbContext.ClientServices.AddAsync(clientService);
            await dbContext.SaveChangesAsync(); // needed to get clientService.Id for checkpoints

            await InitializeCheckpoints(clientService.Id, csDto.SelectedCheckpointIds);

            foreach (var tgDto in csDto.TaskGroups)
            {
                await ProcessTaskGroup(clientService, client, tgDto, user);
            }
        }

        private async Task InitializeCheckpoints(int clientServiceId, List<int>? checkpointIds)
        {
            if (checkpointIds == null || !checkpointIds.Any()) return;

            await checkpointService.CreateClientServiceCheckpointsAsync(clientServiceId, checkpointIds);
        }

        private async Task ProcessTaskGroup(
            Core.Models.ClientService clientService,
            Client client,
            CreateTaskGroupDTO tgDto,
            ApplicationUser user)
        {
            var now = DateTime.Now;
            var taskGroup = new TaskGroup
            {
                ClientService = clientService,
                Month = now.Month,
                Year = now.Year,
                MonthLabel = $"{now:MMMM} {now:yyyy}",
                Tasks = new List<TaskItem>()
            };
            await dbContext.TaskGroups.AddAsync(taskGroup);

            foreach (var taskDto in tgDto.Tasks)
            {
                await ProcessTask(taskDto, taskGroup, clientService, client, user);
            }
        }

        private async Task ProcessTask(
            CreateTaskDTO taskDto,
            TaskGroup taskGroup,
            Core.Models.ClientService clientService,
            Client client,
            ApplicationUser createdBy)
        {
            var employeeId = string.IsNullOrWhiteSpace(taskDto.EmployeeId) ? null : taskDto.EmployeeId;
            var employee = employeeId is not null
                ? await taskHelperService.GetUserOrThrow(employeeId)
                : null;

            var task = new TaskItem
            {
                Title = taskDto.Title,
                TaskType = taskDto.TaskType,
                Description = taskDto.Description,
                Deadline = taskDto.Deadline,
                Priority = taskDto.Priority,
                Refrence = taskDto.Refrence,
                EmployeeId = employeeId,
                Employee = employee,
                TaskGroup = taskGroup,
                ClientService = clientService,
                NumberOfSubTasks = taskDto.NumberOfSubTasks
            };
            await dbContext.Tasks.AddAsync(task);

            await RecordTaskHistory(task, createdBy);
            await DispatchTaskNotifications(task, client, employee);
        }

        private async Task RecordTaskHistory(TaskItem task, ApplicationUser createdBy)
        {
            var history = new TaskItemHistory
            {
                TaskItem = task,
                PropertyName = "انشاء التاسك",
                UpdatedById = createdBy.Id,
                UpdatedByName = $"{createdBy.FirstName} {createdBy.LastName}",
                UpdatedAt = DateTime.UtcNow
            };
            await dbContext.TaskHistory.AddAsync(history);
        }

        private async Task DispatchTaskNotifications(TaskItem task, Client client, ApplicationUser? employee)
        {
            if (employee is null || string.IsNullOrEmpty(employee.Email)) return;

            var emailPayload = new
            {
                To = employee.Email,
                Subject = "New Task Has Been Assigned To You",
                Template = "NewTaskAssignment",
                Replacements = new Dictionary<string, string>
                {
                    { "FullName", $"{employee.FirstName} {employee.LastName}" },
                    { "Email", employee.Email },
                    { "TaskTitle", task.Title },
                    { "TaskType", task.TaskType.ToString() },  // bug fix: was "task.TaskType" string literal
                    { "TaskId", task.Id.ToString() },
                    { "ClientName", client.Name },
                    { "TimeStamp", DateTime.UtcNow.ToString() }
                }
            };
            await taskHelperService.AddOutboxAsync(OutboxTypes.Email, "Send New Task Email", emailPayload);

            if (!string.IsNullOrEmpty(client.DiscordChannelId))
            {
                var discordPayload = new DiscordPayload(client.DiscordChannelId, task, DiscordOperationType.NewTask);
                await taskHelperService.AddOutboxAsync(OutboxTypes.Discord, "Send New Discord Notification", discordPayload);
            }
        }

        private async Task<ClientDTO> ReloadAndMapClient(int clientId)
        {
            var client = await dbContext.Clients
                .Include(c => c.AccountManager)
                .Include(c => c.ClientServices).ThenInclude(cs => cs.Service)
                .Include(c => c.ClientServices).ThenInclude(cs => cs.TaskGroups).ThenInclude(tg => tg.Tasks).ThenInclude(t => t.Employee)
                .Include(c => c.ClientServices).ThenInclude(cs => cs.ClientServiceCheckpoints).ThenInclude(csc => csc.ServiceCheckpoint)
                .Include(c => c.ClientServices).ThenInclude(cs => cs.ClientServiceCheckpoints).ThenInclude(csc => csc.CompletedByEmployee)
                .FirstOrDefaultAsync(c => c.Id == clientId);

            return mapper.Map<ClientDTO>(client!);
        }
    }
}
