using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class TaskConfig : IEntityTypeConfiguration<TaskItem>
    {
        public void Configure(EntityTypeBuilder<TaskItem> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.TaskType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Description)
                .IsRequired(false)
                .HasMaxLength(5000);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.ClientServiceId)
                .IsRequired();

            builder.Property(t => t.EmployeeId)
                .IsRequired(false);

            builder.Property(t => t.Deadline)
                .IsRequired();

            builder.Property(t => t.Refrence)
                .IsRequired(false)
                .HasMaxLength(1000);

            builder.Property(t => t.Priority)
                .IsRequired();

            builder.Property(t => t.NumberOfSubTasks)
                .IsRequired(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationship with TaskGroup
            builder.HasOne(t => t.TaskGroup)
                .WithMany(tg => tg.Tasks)
                .HasForeignKey(t => t.TaskGroupId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade

            // Relationship with ApplicationUser (Employee)
            builder.HasOne(t => t.Employee)
                .WithMany(u => u.TaskItems)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Relationship with ClientService - ADDED
            builder.HasOne(t => t.ClientService)
                .WithMany() // ClientService doesn't have a Tasks collection
                .HasForeignKey(t => t.ClientServiceId)
                .OnDelete(DeleteBehavior.NoAction); // Prevent cascade cycle

            // Add indexes for foreign keys
            builder.HasIndex(t => t.TaskGroupId);
            builder.HasIndex(t => t.EmployeeId);
            builder.HasIndex(t => t.ClientServiceId);
        }
    }
}
