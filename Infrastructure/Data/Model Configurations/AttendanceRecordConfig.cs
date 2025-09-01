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
    public class AttendanceRecordConfig : IEntityTypeConfiguration<AttendanceRecord>
    {
        public void Configure(EntityTypeBuilder<AttendanceRecord> builder)
        {
            builder.HasKey(ar => ar.Id);

            builder.Property(ar => ar.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(ar => ar.EmployeeId)
                .IsRequired();

            builder.Property(ar => ar.CheckInTime)
                .IsRequired();

            builder.Property(ar => ar.Status)
                .IsRequired()
                .HasConversion<string>();

            builder.Property(ar => ar.DeviceId)
                .IsRequired();

            builder.Property(ar => ar.IpAddress)
                .IsRequired();

            builder.HasOne(ar => ar.Employee)
                .WithMany(e => e.AttendanceRecords)
                .HasForeignKey(ar => ar.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(ar => new { ar.EmployeeId, ar.CheckInTime })
                .IsUnique();
        }
    }
}
