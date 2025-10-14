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
    public class BreakConfig : IEntityTypeConfiguration<BreakPeriod>
    {
        public void Configure(EntityTypeBuilder<BreakPeriod> builder)
        {
            builder.HasKey(b => b.Id);

            builder.Property(b => b.Id)
                .UseIdentityColumn(1, 1);

            builder.Property(b => b.AttendanceRecordId)
                .IsRequired();

            builder.Property(b => b.StartTime)
                .IsRequired()
                .HasDefaultValueSql("GETDATE()");

            builder.Property(b => b.EndTime)
                .IsRequired(false);

            // Each BreakPeriod belongs to one AttendanceRecord
            builder.HasOne(b => b.AttendanceRecord)
                .WithMany(ar => ar.BreakPeriods)
                .HasForeignKey(b => b.AttendanceRecordId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(b => b.AttendanceRecordId);
        }
    }
}
