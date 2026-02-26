using Core.Models;
using Infrastructure.Data.Model_Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class MinaretOpsDbContext: IdentityDbContext<ApplicationUser>
    {
        public MinaretOpsDbContext(DbContextOptions<MinaretOpsDbContext> options) : base(options)
        { }
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        public DbSet<EmployeeOnBoardingInvitation> EmployeeOnBoardingInvitations { get; set; }
        public DbSet<JobDescription> JobDescriptions { get; set; }
        public DbSet<JobResponsibility> JobResponsibilities { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ClientService> ClientServices { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<ArchivedTask> ArchivedTasks { get; set; }
        public DbSet<TaskItemHistory> TaskHistory { get; set; }
        public DbSet<TaskCompletionResources> TaskCompletionResources { get; set; }
        public DbSet<TaskComment> TaskComments { get; set; }
        public DbSet<TaskGroup> TaskGroups { get; set; }
        public DbSet<InternalTask> InternalTasks { get; set; }
        public DbSet<InternalTaskAssignment> InternalTaskAssignments { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<BreakPeriod> BreakPeriods { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<AnnouncementLink> AnnouncementLinks { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<KPIIncedint> KPIIncedints { get; set; }
        public DbSet<Outbox> OutboxMessages { get; set; }
        public DbSet<ServiceCheckpoint> ServiceCheckpoints { get; set; }
        public DbSet<ClientServiceCheckpoint> ClientServiceCheckpoints { get; set; }
        public DbSet<SalaryPeriod> SalaryPeriods { get; set; }
        public DbSet<SalaryPayment> SalaryPayments { get; set; }
        public DbSet<Currency> Currencies { get; set; }
        public DbSet<ExchangeRate> ExchangeRates { get; set; }
        public DbSet<CustomContract> Contracts { get; set; }
        public DbSet<Branch> Branches { get; set; }
        public DbSet<Vault> Vaults { get; set; }
        public DbSet<VaultTransaction> VaultTransactions { get; set; }
        public DbSet<SalesLead> SalesLeads { get; set; }
        public DbSet<SalesLeadHistory> LeadHistory { get; set; }
        public DbSet<LeadServices> LeadServices { get; set; }
        public DbSet<LeadNote> LeadNotes { get; set; }
        public DbSet<ContactEntry> ContactEntries { get; set; }
        public DbSet<SeoContent> SeoContents { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations
            builder.ApplyConfiguration(new ClientConfig());
            builder.ApplyConfiguration(new EmployeeInvitationConfig());
            builder.ApplyConfiguration(new ServiceConfig());
            builder.ApplyConfiguration(new ClientServiceConfig());
            builder.ApplyConfiguration(new TaskConfig());
            builder.ApplyConfiguration(new TaskCommentConfig());
            builder.ApplyConfiguration(new ArchivedTaskConfig());
            builder.ApplyConfiguration(new TaskLinksConfig());
            builder.ApplyConfiguration(new TaskHistoryConfig());
            builder.ApplyConfiguration(new TaskGroupConfig());
            builder.ApplyConfiguration(new InternalTaskConfig());
            builder.ApplyConfiguration(new InternalTaskAssignmentConfig());
            builder.ApplyConfiguration(new AttendanceRecordConfig());
            builder.ApplyConfiguration(new LeaveRequestConfig());
            builder.ApplyConfiguration(new AnnouncementConfig());
            builder.ApplyConfiguration(new AnnouncementLinkConfig());
            builder.ApplyConfiguration(new ComplaintConfig());
            builder.ApplyConfiguration(new IncedientConfig());
            builder.ApplyConfiguration(new OutboxConfig());
            builder.ApplyConfiguration(new ServiceCheckpointConfig());
            builder.ApplyConfiguration(new ClientServiceCheckpointConfig());
            builder.ApplyConfiguration(new SalaryPaymentConfig());
            builder.ApplyConfiguration(new SalaryPeriodConfig());
            builder.ApplyConfiguration(new ContractConfig());
            builder.ApplyConfiguration(new CurrencyConfig());
            builder.ApplyConfiguration(new ExchangeRateConfig());
            builder.ApplyConfiguration(new BranchConfig());
            builder.ApplyConfiguration(new VaultConfig());
            builder.ApplyConfiguration(new VaultTransactionConfig());
            builder.ApplyConfiguration(new LeadConfig());
            builder.ApplyConfiguration(new LeadHistoryConfig());
            builder.ApplyConfiguration(new LeadNoteConfig());
            builder.ApplyConfiguration(new LeadServiceConfig());
            builder.ApplyConfiguration(new SeoContentConfig());
            builder.ApplyConfiguration(new ContactFormConfig());
        }
    }
}
