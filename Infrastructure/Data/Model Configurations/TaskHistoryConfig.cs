using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Data.Model_Configurations
{
    public class TaskHistoryConfig : IEntityTypeConfiguration<TaskItemHistory>
    {
        public void Configure(EntityTypeBuilder<TaskItemHistory> builder)
        {
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(h => h.TaskItemId)
                .IsRequired(false);  // Changed from IsRequired() to IsRequired(false)

            builder.Property(h => h.ArchivedTaskId)
                .IsRequired(false);  // ADD THIS

            builder.Property(h => h.PropertyName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(h => h.OldValue)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(h => h.NewValue)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(h => h.UpdatedById)
                .IsRequired(false);

            builder.Property(h => h.PropertyName)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(h => h.UpdatedAt)
                .HasDefaultValueSql("GETDATE()");

            // Relationship with TaskItem (optional, can be null when archived)
            builder.HasOne(h => h.TaskItem)
                .WithMany(t => t.TaskHistory)
                .HasForeignKey(h => h.TaskItemId)
                .OnDelete(DeleteBehavior.SetNull);  // Changed from Cascade to SetNull

            // Relationship with ArchivedTask (optional, used when task is archived)
            builder.HasOne(h => h.ArchivedTask)
                .WithMany(a => a.TaskHistory)
                .HasForeignKey(h => h.ArchivedTaskId)
                .OnDelete(DeleteBehavior.Cascade);  // ADD THIS

            // Relationship with ApplicationUser (UpdatedBy)
            builder.HasOne(h => h.UpdatedBy)
                .WithMany()
                .HasForeignKey(h => h.UpdatedById)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
