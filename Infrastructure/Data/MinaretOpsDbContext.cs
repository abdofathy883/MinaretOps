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
        public DbSet<Client> Clients { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<ClientService> ClientServices { get; set; }
        public DbSet<TaskItem> Tasks { get; set; }
        public DbSet<TaskItemHistory> TaskHistory { get; set; }
        public DbSet<TaskCompletionResources> TaskCompletionResources { get; set; }
        public DbSet<TaskGroup> TaskGroups { get; set; }
        public DbSet<InternalTask> InternalTasks { get; set; }
        public DbSet<InternalTaskAssignment> InternalTaskAssignments { get; set; }
        public DbSet<ContactFormEntry> ContactFormEntries { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<PushNotification> PushNotifications { get; set; }
        public DbSet<CustomPushSubscription> PushSubscriptions { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<EmployeeAnnouncement> EmployeeAnnouncements { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<KPIIncedint> KPIIncedints { get; set; }
        public DbSet<BlogCategory> BlogCategories { get; set; }
        public DbSet<Post> BlogPosts { get; set; }
        public DbSet<ProjectCategory> ProjectCategories { get; set; }
        public DbSet<Project> Projects { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations
            builder.ApplyConfiguration(new ClientConfig());
            builder.ApplyConfiguration(new ServiceConfig());
            builder.ApplyConfiguration(new ClientServiceConfig());
            builder.ApplyConfiguration(new TaskConfig());
            builder.ApplyConfiguration(new TaskLinksConfig());
            builder.ApplyConfiguration(new TaskGroupConfig());
            builder.ApplyConfiguration(new InternalTaskConfig());
            builder.ApplyConfiguration(new InternalTaskAssignmentConfig());
            builder.ApplyConfiguration(new ContactFormConfig());
            builder.ApplyConfiguration(new AttendanceRecordConfig());
            builder.ApplyConfiguration(new LeaveRequestConfig());
            builder.ApplyConfiguration(new AnnouncementConfig());
            builder.ApplyConfiguration(new EmployeeAnnouncementConfig());
            builder.ApplyConfiguration(new ComplaintConfig());
            builder.ApplyConfiguration(new IncedientConfig());
        }
    }
}
