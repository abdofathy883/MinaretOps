using AutoMapper;
using Core.DTOs.Clients;
using Core.Interfaces;
using Core.Models;
using Infrastructure.Data;
using Infrastructure.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.Clients
{
    public class ClientService : IClientServices
    {
        private readonly MinaretOpsDbContext dbContext;
        private readonly IMapper mapper;
        private readonly ILogger<ClientService> logger;
        private readonly IEmailService emailService;
        public ClientService(
            MinaretOpsDbContext minaret,
            IMapper _mapper,
            ILogger<ClientService> _logger,
            IEmailService email
            )
        {
            dbContext = minaret;
            mapper = _mapper;
            logger = _logger;
            emailService = email;
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

        public async Task<ClientDTO> AddClientAsync(CreateClientDTO clientDTO)
        {
            if (clientDTO is null)
                throw new InvalidObjectException("بيانات العميل غير كاملة, برجاء ادخال كافة البيانات المطلوبة واعادة المحاولة");

            var existingClient = await dbContext.Clients
                .AnyAsync(c => c.Name == clientDTO.Name);

            if (existingClient)
                throw new AlreadyExistObjectException("لا يمكن اضافة عميل موجود بالفعل");

            using var dbTransaction = await dbContext.Database.BeginTransactionAsync();
            try
            {
                var newClient = new Client
                {
                    Name = clientDTO.Name,
                    CompanyName = clientDTO.CompanyName,
                    PersonalPhoneNumber = clientDTO.PersonalPhoneNumber,
                    CompanyNumber = clientDTO.CompanyNumber,
                    BusinessDescription = clientDTO.BusinessDescription,
                    DriveLink = clientDTO.DriveLink,
                    ClientServices = new List<Core.Models.ClientService>()
                };

                // First, add the client to get its ID
                await dbContext.AddAsync(newClient);
                await dbContext.SaveChangesAsync();

                // Now create ClientServices with proper relationships
                foreach (var csDto in clientDTO.ClientServices)
                {
                    var clientService = new Core.Models.ClientService
                    {
                        ClientId = newClient.Id,
                        ServiceId = csDto.ServiceId,
                        TaskGroups = new List<TaskGroup>()
                    };

                    await dbContext.AddAsync(clientService);
                    await dbContext.SaveChangesAsync();

                    // Create TaskGroups for this ClientService
                    foreach (var tgDto in csDto.TaskGroups)
                    {
                        var taskGroup = new TaskGroup
                        {
                            ClientServiceId = clientService.Id,
                            Month = DateTime.Now.Month,
                            Year = DateTime.Now.Year,
                            MonthLabel = $"{DateTime.Now.ToString("MMMM")} {DateTime.Now.ToString("yyyy")}",
                            Tasks = new List<TaskItem>()
                        };

                        await dbContext.AddAsync(taskGroup);
                        await dbContext.SaveChangesAsync();

                        // Create Tasks for this TaskGroup
                        foreach (var taskDto in tgDto.Tasks)
                        {
                            var task = new TaskItem
                            {
                                Title = taskDto.Title,
                                TaskType = taskDto.TaskType,
                                Description = taskDto.Description,
                                Deadline = taskDto.Deadline,
                                Priority = taskDto.Priority,
                                Refrence = taskDto.Refrence,
                                EmployeeId = taskDto.EmployeeId,
                                TaskGroupId = taskGroup.Id,
                                ClientServiceId = clientService.Id // This was missing!
                            };

                            await dbContext.AddAsync(task);

                            // Get employee information from the database to avoid null reference
                            var employee = await dbContext.Users.FirstOrDefaultAsync(u => u.Id == taskDto.EmployeeId);
                            if (employee == null)
                            {
                                throw new InvalidObjectException($"Employee with ID {taskDto.EmployeeId} not found");
                            }

                            Dictionary<string, string> replacements = new Dictionary<string, string>
                            {
                                {"FullName", $"{task.Employee.FirstName} {task.Employee.LastName}" },
                                {"Email", $"{task.Employee.Email}" },
                                {"TaskTitle", $"{task.Title}" },
                                {"ClientName", $"{task.ClientService.Client.Name}" },
                                {"TimeStamp", $"{DateTime.UtcNow}" }
                            };

                            await emailService.SendEmailWithTemplateAsync(task.Employee.Email, "New Client Assigned To You", "NewClientAssignment", replacements);
                        }
                    }
                }

                // Final save to persist all tasks
                await dbContext.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return mapper.Map<ClientDTO>(newClient);
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                logger.LogError($"Error adding new client: {ex}");
                throw new NotImplementedOperationException("خطأ في اضافة العميل, حاول مرة اخرى");
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
                .ThenInclude(cs => cs.Service)  // Include the Service entity
            .Include(c => c.ClientServices)
                .ThenInclude(cs => cs.TaskGroups)  // Include TaskGroups from ClientService
                    .ThenInclude(tg => tg.Tasks)   // Include Tasks from TaskGroup
                        .ThenInclude(t => t.Employee)  // Include Employee from Task
            .FirstOrDefaultAsync(c => c.Id == clientId)
                ?? throw new InvalidObjectException("لا يوجد عميل بهذه البيانات");

            return client;
        }
    }
}
