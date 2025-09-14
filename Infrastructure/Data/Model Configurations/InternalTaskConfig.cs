using Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Data.Model_Configurations
{
    public class InternalTaskConfig : IEntityTypeConfiguration<InternalTask>
    {
        public void Configure(EntityTypeBuilder<InternalTask> builder)
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
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(t => t.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(t => t.Priority)
                .IsRequired();

            builder.Property(t => t.Deadline)
                .IsRequired();

            builder.Property(t => t.CreatedAt)
                .IsRequired();

            builder.HasMany(t => t.Assignments)
                .WithOne(a => a.Task)
                .HasForeignKey(a => a.InternalTaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}


