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
    public class AnnouncementConfig : IEntityTypeConfiguration<Announcement>
    {
        public void Configure(EntityTypeBuilder<Announcement> builder)
        {
            builder.HasKey(a => a.Id);

            builder.Property(a => a.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(a => a.Title)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(a => a.Message)
                .IsRequired()
                .HasMaxLength(2000);

            builder.Property(a => a.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("GETUTCDATE()");

            // Configure the one-to-many relationship with EmployeeAnnouncement
            builder.HasMany(a => a.EmployeeAnnouncements)
                .WithOne(ea => ea.Announcement)
                .HasForeignKey(ea => ea.AnnouncementId)
                .OnDelete(DeleteBehavior.Cascade);

            // Add index for better query performance
            builder.HasIndex(a => a.CreatedAt);
        }
    }
}
