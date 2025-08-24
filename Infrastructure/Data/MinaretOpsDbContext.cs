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
        public DbSet<TaskGroup> TaskGroups { get; set; }
        public DbSet<InternalTask> InternalTasks { get; set; }
        public DbSet<InternalTaskAssignment> InternalTaskAssignments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations
            builder.ApplyConfiguration(new ClientConfig());
            builder.ApplyConfiguration(new ServiceConfig());
            builder.ApplyConfiguration(new ClientServiceConfig());
            builder.ApplyConfiguration(new TaskConfig());
            builder.ApplyConfiguration(new TaskGroupConfig());
            builder.ApplyConfiguration(new InternalTaskConfig());
            builder.ApplyConfiguration(new InternalTaskAssignmentConfig());
        }
    }
}
