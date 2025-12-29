using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class ArchivedTaskConfig : IEntityTypeConfiguration<ArchivedTask>
    {
        public void Configure(EntityTypeBuilder<ArchivedTask> builder)
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).UseIdentityColumn(1, 1);

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

            builder.Property(t => t.NumberOfSubTasks)
                .IsRequired(false);

            builder.Property(t => t.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            // Relationships
            builder.HasOne(t => t.TaskGroup)
                .WithMany()
                .HasForeignKey(t => t.TaskGroupId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(t => t.Employee)
                .WithMany()
                .HasForeignKey(t => t.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(t => t.ClientService)
                .WithMany()
                .HasForeignKey(t => t.ClientServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Indexes
            builder.HasIndex(t => t.ClientServiceId);
        }
    }
}