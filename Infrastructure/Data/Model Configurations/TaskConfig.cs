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

            // Configure properties
            builder.Property(t => t.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(t => t.TaskType)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Description)
                .IsRequired(false)
                .HasMaxLength(2000);

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

            builder.Property(t => t.IsArchived)
                .HasDefaultValue(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationship with ClientService
            builder.HasOne(t => t.TaskGroup)
                .WithMany(cs => cs.Tasks)
                .HasForeignKey(t => t.TaskGroupId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relationship with ApplicationUser (Employee)
            builder.HasOne(t => t.Employee)
                .WithMany(u => u.TaskItems)
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            // Add indexes for foreign keys
            builder.HasIndex(t => t.TaskGroupId);
            builder.HasIndex(t => t.EmployeeId);
        }
    }
}
