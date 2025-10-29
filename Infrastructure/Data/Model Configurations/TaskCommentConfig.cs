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
    public class TaskCommentConfig : IEntityTypeConfiguration<TaskComment>
    {
        public void Configure(EntityTypeBuilder<TaskComment> builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(c => c.Comment)
                .IsRequired()
                .HasMaxLength(6000);

            builder.Property(c => c.EmployeeId)
                .IsRequired(false);

            builder.Property(c => c.TaskId)
                .IsRequired();

            builder.HasOne(c => c.Employee)
                .WithMany()
                .HasForeignKey(c => c.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(c => c.Task)
                .WithMany(c => c.TaskComments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
