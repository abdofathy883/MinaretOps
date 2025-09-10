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
    public class EmployeeAnnouncementConfig : IEntityTypeConfiguration<EmployeeAnnouncement>
    {
        public void Configure(EntityTypeBuilder<EmployeeAnnouncement> builder)
        {
            // Configure composite primary key
            builder.HasKey(ea => new { ea.EmployeeId, ea.AnnouncementId });

            builder.Property(ea => ea.EmployeeId)
                .IsRequired();

            builder.Property(ea => ea.AnnouncementId)
                .IsRequired();

            builder.Property(ea => ea.IsRead)
                .IsRequired()
                .HasDefaultValue(false);

            // Configure the many-to-one relationship with ApplicationUser
            builder.HasOne(ea => ea.Employee)
                .WithMany(ea => ea.EmployeeAnnouncements)
                .HasForeignKey(ea => ea.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure the many-to-one relationship with Announcement
            builder.HasOne(ea => ea.Announcement)
                .WithMany(a => a.EmployeeAnnouncements)
                .HasForeignKey(ea => ea.AnnouncementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add indexes for better query performance
            builder.HasIndex(ea => ea.EmployeeId);
            builder.HasIndex(ea => ea.AnnouncementId);
            builder.HasIndex(ea => new { ea.EmployeeId, ea.IsRead });
        }
    }
}
