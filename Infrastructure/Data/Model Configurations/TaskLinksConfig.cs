using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class TaskLinksConfig : IEntityTypeConfiguration<TaskCompletionResources>
    {
        public void Configure(EntityTypeBuilder<TaskCompletionResources> builder)
        {
            builder.HasKey(r => r.Id);

            builder.Property(r => r.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(r => r.URL)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(r => r.TaskId)
                .IsRequired(false);  // Changed from IsRequired() to IsRequired(false)

            builder.Property(r => r.ArchivedTaskId)
                .IsRequired(false);  // ADD THIS

            // Relationship with TaskItem (optional, can be null when archived)
            builder.HasOne(r => r.Task)
                .WithMany(t => t.CompletionResources)
                .HasForeignKey(r => r.TaskId)
                .OnDelete(DeleteBehavior.SetNull);  // ADD THIS - Changed from Cascade to SetNull

            // Relationship with ArchivedTask (optional, used when task is archived)
            builder.HasOne(r => r.ArchivedTask)
                .WithMany(a => a.CompletionResources)
                .HasForeignKey(r => r.ArchivedTaskId)
                .OnDelete(DeleteBehavior.Cascade);  // ADD THIS
        }
    }
}
