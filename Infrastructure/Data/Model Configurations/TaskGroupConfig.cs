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
    public class TaskGroupConfig : IEntityTypeConfiguration<TaskGroup>
    {
        public void Configure(EntityTypeBuilder<TaskGroup> builder)
        {
            builder.HasKey(tg => tg.Id);

            builder.Property(tg => tg.Id)
                .UseIdentityColumn(1, 1);

            // Configure properties
            builder.Property(tg => tg.Month)
                .IsRequired();

            builder.Property(tg => tg.Year)
                .IsRequired();

            builder.Property(tg => tg.MonthLabel)
                .IsRequired()
                .HasMaxLength(50);

            // Relationship with ClientService
            builder.HasOne(tg => tg.ClientService)
                .WithMany(cs => cs.TaskGroups)
                .HasForeignKey(tg => tg.ClientServiceId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add index for foreign key
            builder.HasIndex(tg => tg.ClientServiceId);
        }
    }
}
